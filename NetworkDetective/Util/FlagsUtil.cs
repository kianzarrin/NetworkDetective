namespace NetworkDetective.Util {
    using KianCommons;
    using KianCommons.Plugins;
    using System.Collections.Generic;

    internal static class FlagsUtil {
        static string FlagsToString(object a, object b) {
            string reta = a?.ToString() ?? "";
            string retb = b?.ToString() ?? "";
            if (reta != "" && retb != "") {
                return reta + ", " + retb;
            } else {
                return reta + retb;
            }
        }


        internal static string ArrayToString(string[] ar) {
            string ret = "";
            if (ar != null) {
                bool first = true;
                foreach (string item in ar) {
                    if (!first) ret += ", ";
                    ret += item;
                    first = false;
                }
            }
            return ret;
        }

        public static string GetNodeTags(ushort nodeID) {
            string []tags = NetUtil.GetTags(nodeID.ToNode().m_tags);
            return ArrayToString(tags);
        }

        /// <summary>
        /// camma seperated list of tag(count)
        /// </summary>
        public static string GetNodeCombinedTags(ushort nodeID) {
            Dictionary<string, int> tags = new(); // tag name -> occurance count
            foreach(ushort segmentId  in nodeID.ToNode().IterateSegments()) {
                foreach (var tag in segmentId.ToSegment().Info.m_tags) {
                    if (!tags.ContainsKey(tag)) {
                        tags[tag] = 0;
                    }
                    tags[tag]++;
                }
            }

            string ret = "";
            bool first = true;
            foreach (var pair in tags) {
                if (!first) ret += ", ";
                ret += $"{pair.Key}({pair.Value})";
                first = false;
            }


            return ret;
        }

        public static string GetNodeFlags(ushort nodeID) {
            var a = nodeID.ToNode().flags;
            var b = AdaptiveRoadsUtil.GetARNodeFlags(nodeID);
            return FlagsToString(a, b);
        }
        public static string GetSegmentFlags(ushort segmentId) {
            var a = segmentId.ToSegment().m_flags;
            var b = AdaptiveRoadsUtil.GetARSegmentFlags(segmentId);
            return FlagsToString(a, b);
        }

        public static string GetLaneFlags(uint laneID) {
            var a = laneID.ToLane().Flags();
            var b = AdaptiveRoadsUtil.GetARLaneFlags(laneID);
            return FlagsToString(a, b);
        }

        public static string GetSegmentEndFlags(ushort segmentId, bool startNode) {
            var flags = AdaptiveRoadsUtil.GetARSegmentEndFlags(segmentId, startNode);
            string title = startNode ? "Start" : "End";
            return $"Segment{title}: {flags}";
        }
    }
}
