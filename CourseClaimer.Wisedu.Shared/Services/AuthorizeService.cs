using System.Net;
using System.Security.Cryptography;
using System.Text;
using CourseClaimer.Ocr;
using CourseClaimer.Wisedu.Shared.Enums;
using CourseClaimer.Wisedu.Shared.Extensions;
using CourseClaimer.Wisedu.Shared.Models.Database;
using CourseClaimer.Wisedu.Shared.Models.JWXK.Roots;
using CourseClaimer.Wisedu.Shared.Models.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseClaimer.Wisedu.Shared.Services
{
    public class AuthorizeService(Aes aesUtil, OcrService ocr, IHttpClientFactory clientFactory,IConfiguration configuration,IServiceProvider serviceProvider)
    {
        public async Task<LoginResult> MakeUserLogin(Entity entity,bool IsReLogin = false)
        {
            var dbContext = serviceProvider.GetRequiredService<ClaimDbContext>();
            dbContext.EntityRecords.Add(new EntityRecord()
            {
                UserName = entity.username,
                Message = $"MakeUserLogin: {entity.username} entered with IsReLogin={IsReLogin}"
            });
            await dbContext.SaveChangesAsync();
            if (IsReLogin) await Task.Delay(Convert.ToInt32(configuration["ReLoginDelayMilliseconds"]));
            entity.finished = false;
            entity.client = clientFactory.CreateClient("JWXK");
            var captcha = await entity.Captcha().ToResponseDto<CaptchaRoot>();
            captcha.EnsureSuccess();
            var authCode = ocr.classification(img_base64: captcha!.Data.data.captcha.Split(',')[1]);
            var login = await entity.Login(AESEncrypt(entity.password), captcha.Data.data.uuid, authCode).ToResponseDto<LoginRoot>();
            if (login.InnerMessage.Contains("密码错误")) return LoginResult.WrongPassword;
            if (login.InnerMessage.Contains("验证码")) return LoginResult.WrongCaptcha;
            switch (login.InnerCode)
            {
                case HttpStatusCode.InternalServerError:
                    return LoginResult.UnknownError;
                case HttpStatusCode.OK:
                    entity.batchId = login.Data.data.student.hrbeuLcMap.First().Key;
                    entity.client.DefaultRequestHeaders.Authorization = new(login.Data.data.token);
                    entity.client.DefaultRequestHeaders.Add("Cookie", $"Authorization={login.Data.data.token}");
                    entity.client.DefaultRequestHeaders.Add("batchId", entity.batchId);
                    return LoginResult.Success;
                default:
                    login.EnsureSuccess();
                    return LoginResult.UnknownError;
            }
        }

        private string AESEncrypt(string text)
        {
            var cipher = aesUtil.EncryptEcb(Encoding.UTF8.GetBytes(text), PaddingMode.PKCS7);
            return Convert.ToBase64String(cipher);
        }
    }
}
