using MpgCustom;
using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using RealtimeViewer.Network;
using RealtimeViewer.Model;
using System.Collections;

namespace RealtimeViewer
{
    public class MainViewModel
    {
        /// <summary>
        /// マップルマップに対し使用するスケール。
        /// </summary>
        public int[] MapScales { get; private set; }

        public Point[] BalloonOffsets;
        public Random Random { get; private set; }

        public OperationServerInfo ServerInfo { get; set; }

        /// <summary>
        /// 地図に描画しているオブジェクト(管理用)
        /// </summary>
        public Dictionary<string, MapEntryInfo> MapEntries { get; set; }
        /// <summary>
        /// 地図に描画しているオブジェクト(描画用)
        /// </summary>
        public ArrayList MapEntriesForMpgMap { get; set; }

        public SortableBindingList<MapEntryInfo> BindingCarList { get; set; }

        public SortableBindingList<CarInfo> RecieveErrorList { get; set; }

        public MainViewModel()
        {
            // マップルのスケールは1ステップが異様に細かい。
            // 適度に間引いたスケールを右側のバーに対して使う。
            // マップルにおけるスケールの範囲は1000から5百万。
            MapScales = new int[]
            {
                1000, 2500, 5000,
                10000, 25000, 50000,
                100000, 250000, 500000,
                1000000, 2500000, 5000000,
            };

            const int OFFSET_COUNT = 64;
            const int OFFSET_R = 64;
            var p = new List<System.Drawing.Point>();
            for (int n = 0; n < OFFSET_COUNT; n++)
            {
                double x = OFFSET_R * Math.Cos(n * 2 * Math.PI / OFFSET_COUNT);
                double y = OFFSET_R * Math.Sin(n * 2 * Math.PI / OFFSET_COUNT);
                p.Add(new System.Drawing.Point((int)(x + 0.5), (int)(y + 0.5)));
            }
            BalloonOffsets = p.ToArray();

            Random = new Random();
            MapEntries = new Dictionary<string, MapEntryInfo>();
            MapEntriesForMpgMap = new ArrayList();
            RecieveErrorList = new SortableBindingList<CarInfo>();
            BindingCarList = new SortableBindingList<MapEntryInfo>();
        }

        public int GetNearIndexInMapScales(int x)
        {
            int result = MapScales.Length - 1;
            for (int index = 1; index < MapScales.Length; index++)
            {
                if (x <= MapScales[index])
                {
                    int c = (MapScales[index - 1] + MapScales[index]) / 2;
                    if (x <= c)
                    {
                        result = index - 1;
                    }
                    else
                    {
                        result = index;
                    }
                    break;
                }
            }

            return result;
        }

        public Point GetRandomBalloonOffset()
        {
            return BalloonOffsets[Random.Next(BalloonOffsets.Length)];
        }
    }
}
