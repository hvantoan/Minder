using Minder.Database.Enums;
using Minder.Service.Extensions;
using Minder.Service.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minder.Service.Models.Common {

    public class TimeOpptionDto {
        public EDayOfWeek DayOfWeek { get; set; }
        public string DisplayDay => DayOfWeek.Description();
        public DateTimeOffset Date { get; set; }
        public List<TimeItemDto>? Opptions { get; set; }

        public static TimeOpptionDto FromTimeChooice(TimeChooice time) {
            var listConsecutive = CutIntoConsecutiveLists(time.Value);

            return new TimeOpptionDto {
                DayOfWeek = time.Day,
                Date = GetDateFromDayOfWeek(time.Day),
                Opptions = listConsecutive.Select(o => new TimeItemDto() {
                    From = o.Min(),
                    To = o.Max(),
                    MemberCount = time.Quantity
                }).Where(o => o.To - o.From > 0).ToList()
            };
        }

        private static List<List<int>> CutIntoConsecutiveLists(List<int> inputList) {
            List<List<int>> resultLists = new();
            List<int> tempList = new();

            foreach (int num in inputList) {
                if (tempList.Count == 0 || num == tempList[tempList.Count - 1] + 1) {
                    tempList.Add(num);
                } else {
                    resultLists.Add(new List<int>(tempList));
                    tempList.Clear();
                    tempList.Add(num);
                }
            }

            if (tempList.Count > 0) {
                resultLists.Add(new List<int>(tempList));
            }

            return resultLists;
        }

        private static DateTimeOffset GetDateFromDayOfWeek(EDayOfWeek dayOfWeek) {
            var now = DateTimeOffset.Now;

            int stick = (int)now.DayOfWeek - (int)dayOfWeek;
            if (stick > 0 && (int)dayOfWeek != (int)now.DayOfWeek) stick -= 6;
            if (stick < 0) stick *= -1;

            var res = now.AddDays(stick + 1);
            return res;
        }
    }

    public class TimeItemDto {
        public int? From { get; set; }
        public int? To { get; set; }
        public string DisplayTime => TimeToString(this.From ?? 0, this.To ?? 0);
        public int MemberCount { get; set; }

        private static string TimeToString(int from, int to) {
            to++;
            return $"{(from < 10 ? $"0{from}" : from)}:00h - {(to < 10 ? $"0{to}" : to)}:00h";
        }
    }
}