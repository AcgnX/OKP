﻿using OKP.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OKP.Core.Interface.TorrentContent;

namespace OKP.Core.Interface.Dmhy
{
    internal class DmhyAdapter : AdapterBase
    {
        private HttpClient httpClient { get; init; }
        private List<string> Trackers => new();
        private string BaseUrl => "https://share.dmhy.org/";
        private string PingUrl { get => throw new NotImplementedException(); }
        private string PostUtl { get => throw new NotImplementedException(); }

        public DmhyAdapter(TorrentContent torrent,Template template)
        {
            httpClient = new() { 
                BaseAddress=new(BaseUrl)
            };
            this.template = template;
            this.torrent = torrent;
            if(template == null)
            {
                return;
            }
            httpClient.DefaultRequestHeaders.Add("UserAgent", template.UserAgent);
            httpClient.DefaultRequestHeaders.Add("Cookie", template.Cookie);
        }

        public override async Task<HttpResult> PingAsync()
        {
            var pingReq = await httpClient.GetAsync(PingUrl);
            return new((int)pingReq.StatusCode, "", pingReq.IsSuccessStatusCode);
        }

        public override Task<HttpResult> PostAsync()
        {
            httpClient.BaseAddress = new(BaseUrl);
            MultipartFormDataContent form = new()
            {
                { new StringContent("2"), "sort_id" },
                { new StringContent("2"), "team_id" },
                { new StringContent("2"), "bt_data_title" },
                { new StringContent("2"), "poster_url" },
                { new StringContent(CompileTemplate()), "bt_data_intro" },
                { new StringContent("2"), "tracker" },
                { new StringContent("2"), "MAX_FILE_SIZE" },
                { new StringContent("2"), "bt_file", "[SBSUB&LoliHouse] Detective Conan Hannin No Hanzawa San - 02 [WebRip 1080" },
                { new StringContent("2"), "disable_download_seed_file" },
                { new StringContent("2"), "emule_resource" },
                { new StringContent("2"), "synckey" },
                { new StringContent("2"), "submit" }
            };
            httpClient.PostAsyncWithRetry(PostUtl,form);
            throw new NotImplementedException();
        }
        
    }
}
