﻿using BencodeNET.Objects;
using BencodeNET.Parsing;
using BencodeNET.Torrents;
using OKP.Core.Interface;
using OKP.Core.Interface.Bangumi;
using OKP.Core.Interface.Dmhy;

namespace OKP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0] is null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            var torrent = TorrentContent.Build(args[0]);
            Console.WriteLine(torrent.DisplayName);
            if (torrent.IsV2())
            {
                Console.WriteLine("V2达咩！回去换！");
                Console.ReadLine();
                return;
            }
            torrent.DisplayFiles();
            List<AdapterBase> adapterList = new ();
            if(torrent.IntroTemplate is null)
            {
                Console.WriteLine("没有配置发布站你发个啥？");
                Console.ReadLine();
                return;
            }
            foreach (var site in torrent.IntroTemplate)
            {
                if(site.Site is null)
                {
                    Console.WriteLine("没有配置发布站你发个啥？");
                    Console.ReadLine();
                    return;
                }
                AdapterBase adapter = site.Site.ToLower() switch
                {
                    "dmhy" => new DmhyAdapter(torrent, site),
                    "bangumi" => new BangumiAdapter(),
                    _ => throw new NotImplementedException()
                };
                adapterList.Add(adapter);
            }
            List<Task<HttpResult>> PingTask = new();
            adapterList.ForEach(p=>PingTask.Add(p.PingAsync()));
            var PingRes = Task.WhenAll(PingTask).Result;
            foreach (var res in PingRes)
            {
                if (!res.IsSuccess)
                {
                    Console.WriteLine(res.Code+"\t"+res.Message);
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine(res.Code + "\t" + res.Message);
                Console.ReadLine();
            }
        }
    }
}