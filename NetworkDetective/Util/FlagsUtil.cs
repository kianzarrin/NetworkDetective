namespace NetworkDetective.Util {
    using KianCommons;
    using KianCommons.Plugins;

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

        public static string GetNodeFlags(ushort nodeID) {
            var a = nodeID.ToNode().m_flags;
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
