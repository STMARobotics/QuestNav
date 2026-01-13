using Newtonsoft.Json;
using Wpi.Proto;

namespace QuestNav.QuestNav.Geometry
{
    public class Rotation3d
    {
        [JsonProperty("quaternion")]
        public Quaternion Quaternion { get; set; }
        
        [JsonConstructor]
        public Rotation3d(Quaternion quaternion)
        {
            Quaternion = quaternion;
        }
        
        public Rotation3d()
        {
            Quaternion = new Quaternion();
        }

        public Rotation3d(ProtobufRotation3d protobuf)
        {
            Quaternion = new Quaternion(protobuf.Q);
        }

        public ProtobufRotation3d ToProtobuf()
        {
            var proto = new ProtobufRotation3d
            {
                Q = Quaternion.ToProtobuf(),
            };
            return proto;
        }
        
        public Rotation3d RotateBy(Rotation3d other) {
            return new Rotation3d(other.Quaternion.Times(Quaternion));
        }
        
        public Rotation3d Plus(Rotation3d other) {
            return RotateBy(other);
        }
        
        public Rotation3d Minus(Rotation3d other) {
            return RotateBy(other.UnaryMinus());
        }
        
        public Rotation3d UnaryMinus() {
            return new Rotation3d(Quaternion.Inverse());
        }
        
        public override string ToString()
        {
            return $"Rotation3d({Quaternion})";
        }
    }
}
