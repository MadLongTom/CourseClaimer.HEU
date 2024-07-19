﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CourseClaimer.HEU.Shared.Handlers
{
    public class MapForwarderHandler(IConfiguration configuration) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.AbsolutePath == "/xsxk/auth/login")
            {
                request.RequestUri = new Uri(configuration["AuthPath"]);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
