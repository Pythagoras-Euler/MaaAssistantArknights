// <copyright file="StageManager.cs" company="MaaAssistantArknights">
// MaaWpfGui - A part of the MaaCoreArknights project
// Copyright (C) 2021 MistEO and Contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MaaWpfGui
{
    /// <summary>
    /// Stage manager
    /// </summary>
    public class StageManager
    {
        private readonly Dictionary<string, StageInfo> _stages;

        /// <summary>
        /// Initializes a new instance of the <see cref="StageManager"/> class.
        /// </summary>
        public StageManager()
        {
            var sideStory = new StageActivityInfo()
            {
                Tip = "SideStory「将进酒」复刻活动",
                StageName = "IW",
                UtcStartTime = new DateTime(2023, 1, 1, 16, 0, 0).AddHours(-8),
                UtcExpireTime = new DateTime(2023, 1, 11, 4, 0, 0).AddHours(-8),
            };

            var resourceCollection = new StageActivityInfo()
            {
                Tip = "「感谢庆典」，“资源收集”限时全天开放",
                UtcStartTime = new DateTime(2022, 11, 15, 16, 0, 0).AddHours(-8),
                UtcExpireTime = new DateTime(2022, 11, 29, 4, 0, 0).AddHours(-8),
                IsResourceCollection = true,
            };

            _stages = new Dictionary<string, StageInfo>
            {
                // 这里会被 “剩余理智” 复用，第一个必须是 string.Empty 的
                // 「当前/上次」关卡导航
                { string.Empty, new StageInfo { Display = Localization.GetString("DefaultStage"), Value = string.Empty } },

                // SideStory「将进酒」复刻活动
                { "IW-8", new StageInfo { Display = "IW-8", Value = "IW-8", Drop = "30063", Activity = sideStory } },
                { "IW-7", new StageInfo { Display = "IW-7", Value = "IW-7", Drop = "30013", Activity = sideStory } },
                { "IW-6", new StageInfo { Display = "IW-6", Value = "IW-6", Drop = "30103", Activity = sideStory } },

                // 主线关卡
                { "1-7", new StageInfo { Display = "1-7", Value = "1-7" } },

                // 资源本
                { "CE-6", new StageInfo("CE-6", "CETip", new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday }, resourceCollection) },
                { "AP-5", new StageInfo("AP-5", "APTip", new[] { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday }, resourceCollection) },
                { "CA-5", new StageInfo("CA-5", "CATip", new[] { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Sunday }, resourceCollection) },
                { "LS-6", new StageInfo("LS-6", "LSTip", new DayOfWeek[] { }, resourceCollection) },

                // 碳本没做导航，只显示关卡提示
                { "SK-5", new StageInfo("SK-5", "SKTip", new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday }, resourceCollection) { IsHidden = true } },

                // 剿灭模式
                { "Annihilation", new StageInfo { Display = Localization.GetString("Annihilation"), Value = "Annihilation" } },

                // 芯片本
                { "PR-A-1", new StageInfo("PR-A-1", "PR-ATip", new[] { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Sunday }, resourceCollection) },
                { "PR-A-2", new StageInfo("PR-A-2", string.Empty, new[] { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Sunday }, resourceCollection) },
                { "PR-B-1", new StageInfo("PR-B-1", "PR-BTip", new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Friday, DayOfWeek.Saturday }, resourceCollection) },
                { "PR-B-2", new StageInfo("PR-B-2", string.Empty, new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Friday, DayOfWeek.Saturday }, resourceCollection) },
                { "PR-C-1", new StageInfo("PR-C-1", "PR-CTip", new[] { DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday }, resourceCollection) },
                { "PR-C-2", new StageInfo("PR-C-2", string.Empty, new[] { DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday }, resourceCollection) },
                { "PR-D-1", new StageInfo("PR-D-1", "PR-DTip", new[] { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Saturday, DayOfWeek.Sunday }, resourceCollection) },
                { "PR-D-2", new StageInfo("PR-D-2", string.Empty, new[] { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Saturday, DayOfWeek.Sunday }, resourceCollection) },

                // 周一和周日的关卡提示
                { "Pormpt1", new StageInfo { Tip = Localization.GetString("Pormpt1"), OpenDays = new[] { DayOfWeek.Monday }, IsHidden = true } },
                { "Pormpt2", new StageInfo { Tip = Localization.GetString("Pormpt2"), OpenDays = new[] { DayOfWeek.Sunday }, IsHidden = true } },

                // 老版本「当前/上次」关卡导航
                // new StageInfo { Display = Localization.GetString("CurrentStage"), Value = string.Empty },
                // new StageInfo { Display = Localization.GetString("LastBattle"), Value = "LastBattle" },
            };

            var language = ViewStatusStorage.Get("GUI.Localization", Localization.DefaultLanguage);
            string filename = string.Empty;
            if (language == "zh-cn")
            {
                filename = Directory.GetCurrentDirectory() + "\\resource\\item_index.json";
            }
            else if (language == "pallas")
            {
                // DoNothing
            }
            else
            {
                Dictionary<string, string> client = new Dictionary<string, string>
                {
                    { "zh-tw", "txwy" },
                    { "en-us", "YoStarEN" },
                    { "ja-jp", "YoStarJP" },
                    { "ko-kr", "YoStarKR" },
                };
                filename = Directory.GetCurrentDirectory() + "\\resource\\global\\" + client[language] + "\\resource\\item_index.json";
            }

            if (File.Exists(filename))
            {
                try
                {
                    data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(filename));
                }
                catch
                {
                    // DoNothing
                }
            }
        }

        /// <summary>
        /// Gets stage by name
        /// </summary>
        /// <param name="stage">Stage name</param>
        /// <returns>Stage info</returns>
        public StageInfo GetStageInfo(string stage)
        {
            _stages.TryGetValue(stage, out var stageInfo);
            return stageInfo;
        }

        /// <summary>
        /// Determine whether stage is open
        /// </summary>
        /// <param name="stage">Stage name</param>
        /// <param name="dayOfWeek">Current day of week</param>
        /// <returns>Whether stage is open</returns>
        public bool IsStageOpen(string stage, DayOfWeek dayOfWeek)
        {
            return GetStageInfo(stage)?.IsStageOpen(dayOfWeek) == true;
        }

        private readonly Dictionary<string, Dictionary<string, string>> data;

        /// <summary>
        /// Gets open stage tips at specified day of week
        /// </summary>
        /// <param name="dayOfWeek">Day of week</param>
        /// <returns>Open stages</returns>
        public string GetStageTips(DayOfWeek dayOfWeek)
        {
            var builder = new StringBuilder();
            var sideStoryFlag = true;
            foreach (var item in _stages)
            {
                if (item.Value.IsStageOpen(dayOfWeek))
                {
                    if (sideStoryFlag && !string.IsNullOrEmpty(item.Value.Activity?.StageName))
                    {
                        DateTime dateTime = DateTime.UtcNow;
                        var daysleftopen = (item.Value.Activity.UtcExpireTime - dateTime).Days;
                        builder.AppendLine(item.Value.Activity.StageName
                            + " "
                            + Localization.GetString("Daysleftopen")
                            + (daysleftopen > 0 ? daysleftopen.ToString() : Localization.GetString("LessThanOneDay")));
                        sideStoryFlag = false;
                    }

                    if (!string.IsNullOrEmpty(item.Value.Tip))
                    {
                        builder.AppendLine(item.Value.Tip);
                    }

                    if (!string.IsNullOrEmpty(item.Value.Drop))
                    {
                        if (data != null && data.ContainsKey(item.Value.Drop))
                        {
                            builder.AppendLine(item.Value.Display + ": " + data[item.Value.Drop]["name"]);
                        }
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets open stage list at specified day of week
        /// </summary>
        /// <param name="dayOfWeek">Day of week</param>
        /// <returns>Open stage list</returns>
        public IEnumerable<CombData> GetStageList(DayOfWeek dayOfWeek)
        {
            return _stages.Values.Where(stage => !stage.IsHidden && stage.IsStageOpen(dayOfWeek));
        }

        /// <summary>
        /// Gets all stage list
        /// </summary>
        /// <returns>All stage list</returns>
        public IEnumerable<CombData> GetStageList()
        {
            return _stages.Values.Where(stage => !stage.IsHidden);
        }
    }
}
