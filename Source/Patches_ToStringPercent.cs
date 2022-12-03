using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DoneMeansDone {
    [HarmonyPatch(typeof(GenText))]
    public static class Patches_ToStringPercent {
        private static readonly float[] Max = new float[] { .99f, .999f, .9999f, .99999f };

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GenText.ToStringPercent), typeof(float))]
        public static void ToStringPercent(ref float f) {
            if (f < 1f && f > .99f) f = .99f;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GenText.ToStringPercent), typeof(float), typeof(string))]
        public static void ToStringPercent_Format(ref float f, string format) {
            try {
                if (f < 1f) {
                    int decimals = 0;
                    if (format.StartsWith("F")) {
                        decimals = int.Parse(format.Substring(1));
                    } else if (format.StartsWith("0")) { 
                        decimals = format.LastIndexOf('#') - format.IndexOf('#') + 1;
                    } else {
                        return;
                    }
                    if (decimals >= 0 && decimals < Max.Length) {
                        float max = Max[decimals];
                        if (f < 1f && f > max) f = max;
                    }
                }
            } catch {
                // If we cause an exception, just don't change it.
            }
        }
    }
}
