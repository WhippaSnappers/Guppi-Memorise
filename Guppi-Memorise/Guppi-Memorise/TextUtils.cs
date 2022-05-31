using System;
using System.Collections.Generic;
using System.Linq;

namespace Guppi_Memorise
{
    public static class TextUtils
    {
        public static List<List<string>> ParseUsersText(string text)
        {
            List<string> t = text.Split('\n').Where(i => i != "").ToList();

            if (isPoem(t))
            {
                //t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 4).Select(x => x.Select(v => v.value).ToList());

                return t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 4).Select(x => x.Select(v => v.value).ToList()).ToList();
            }
            else
            {
                t = text.Split(' ', '\n').Where(i => i != "").ToList();
                List<List<string>> result = t.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 20).Select(x => x.Select(v => v.value).ToList()).ToList();
                result.ForEach(i => {
                    List<List<string>> temp = i.Select((x, inx) => new { index = inx, value = x }).GroupBy(x => x.index / 5).Select(x => x.Select(v => v.value).ToList()).ToList();
                    i.Clear();
                    for (int j = 0; j < temp.Count; j++)
                    {
                        i.Add(String.Join(" ", temp[j]));
                    }
                });
                if (result.Last().Count == 1 && result.Count >= 2)
                {
                    result[result.Count - 2].Add(result.Last()[0]);
                    result.Remove(result.Last());
                }

                return result;
            }
        }
        public static bool isPoem(List<string> text)
        {
            bool isPoem = true;
            foreach (var line in text)
            {
                if (line.Length >= 50)
                {
                    isPoem = false;
                    break;
                }
            }
            return isPoem;
        }
    }
}
