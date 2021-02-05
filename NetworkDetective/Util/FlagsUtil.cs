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
            var start = AdaptiveRoadsUtil.GetARSegmentEndFlags(segmentId, true);
            var end = AdaptiveRoadsUtil.GetARSegmentEndFlags(segmentId, false);
#pragma warning disable HAA0201 // Implicit string concatenation allocation
            return FlagsToString(a, b)
                + "\nStart: " + start
                + "\nEnd: " + end;
#pragma warning restore HAA0201 // Implicit string concatenation allocation
        }

        public static string GetLaneFlags(uint laneID) {
            var a = laneID.ToLane().Flags();
            var b = AdaptiveRoadsUtil.GetARLaneFlags(laneID);
            return FlagsToString(a, b);
        }
    }
}
