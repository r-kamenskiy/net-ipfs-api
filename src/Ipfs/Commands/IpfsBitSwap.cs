﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Ipfs.Commands
{
    public class IpfsBitSwap : IpfsCommand
    {
        public IpfsBitSwap(Uri commandUri, HttpClient httpClient) : base(commandUri, httpClient)
        {
        }

        /// <summary>
        /// Show some diagnostic information on the bitswap agent
        /// </summary>
        /// <returns>IpfsBitSwapStat object</returns>
        public async Task<IpfsBitSwapStat> Stat()
        {
            HttpContent content = await ExecuteGetAsync("stat", null, null);

            string json = await content.ReadAsStringAsync();

            Json.IpfsBitSwapStat stat = JsonConvert.DeserializeObject<Json.IpfsBitSwapStat>(json);

            return new IpfsBitSwapStat
            {
                ProvideBufLen = stat.ProvideBufLen,
                Wantlist = stat.Wantlist,
                Peers = stat.Peers == null ? null : stat.Peers.Select(x => new MultiHash(x)).ToList(),
                BlocksReceived = stat.BlocksReceived,
                DupBlksReceived = stat.DupBlksReceived,
                DupDataReceived = stat.DupDataReceived,
            };
        }

        /// <summary>
        /// Remove a given block from your wantlist
        /// </summary>
        /// <param name="key">key to remove from your wantlist</param>
        /// <returns></returns>
        public async Task<HttpContent> Unwant(string key)
        {
            return await ExecuteGetAsync("unwant", ToEnumerable(key), null);
        }

        /// <summary>
        /// Show blocks currently on the wantlist
        /// 
        /// Print out all blocks currently on the bitswap wantlist for the local peer
        /// </summary>
        /// <param name="peer">specify which peer to show wantlist for (default self)</param>
        /// <returns></returns>
        public async Task<HttpContent> Wantlist(string peer = null)
        {
            var flags = new Dictionary<string, string>();

            if(!String.IsNullOrEmpty(peer))
            {
                flags.Add("peer", peer);
            }

            return await ExecuteGetAsync("wantlist", null, flags);
        }
    }
}
