using Newtonsoft.Json;
using Wpi.Proto;

namespace QuestNav.QuestNav.Geometry
{
    public class Pose3d
    {
        [JsonProperty("translation")]
        public Translation3d Translation { get; set; }
        [JsonProperty("rotation")]
        public Rotation3d Rotation { get; set; }
        
        public static readonly Pose3d Zero = new Pose3d(new Translation3d(0, 0, 0), new Rotation3d());
        
        [JsonConstructor]
        public Pose3d(Translation3d translation, Rotation3d rotation)
        {
            Translation = translation;
            Rotation = rotation;
        }

        public Pose3d(ProtobufPose3d protobuf)
        {
            Translation = new Translation3d(protobuf. Translation);
            Rotation = new Rotation3d(protobuf.Rotation);
        }
        
        public Pose3d Plus(Transform3d other) {
            return TransformBy(other);
        }
        
        public Pose3d TransformBy(Transform3d other) {
            return new Pose3d(
                Translation.Plus(other.Translation.RotateBy(Rotation)),
                other.Rotation.Plus(Rotation));
        }

        public ProtobufPose3d ToProtobuf()
        {
            var proto = new ProtobufPose3d
            {
                Translation = Translation. ToProtobuf(),
                Rotation = Rotation.ToProtobuf(),
            };
            return proto;
        }

        public override string ToString()
        {
            return $"Pose3d(Translation: {Translation}, Rotation: {Rotation})";
        }
    }
}
