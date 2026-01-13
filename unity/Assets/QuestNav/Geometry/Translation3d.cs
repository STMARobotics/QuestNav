using Newtonsoft.Json;
using Wpi. Proto;

namespace QuestNav.QuestNav.Geometry
{
    public class Translation3d
    {
        [JsonProperty("x")]
        public double X { get; set; }
        [JsonProperty("y")]
        public double Y { get; set; }
        [JsonProperty("z")]
        public double Z { get; set; }
        
        [JsonConstructor]
        public Translation3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Translation3d(ProtobufTranslation3d protobuf)
        {
            X = protobuf.X;
            Y = protobuf.Y;
            Z = protobuf.Z;
        }

        /// <summary>
        /// Applies a rotation to the translation in 3D space. 
        /// For example, rotating a Translation3d of (2, 0, 0) by 90 degrees around the Z axis
        /// will return a Translation3d of (0, 2, 0).
        /// </summary>
        /// <param name="other">The rotation to rotate the translation by.</param>
        /// <returns>The new rotated translation.</returns>
        public Translation3d RotateBy(Rotation3d other)
        {
            var p = new Quaternion(0.0, X, Y, Z);
            var qprime = other.Quaternion.Times(p).Times(other.Quaternion.Inverse());
            return new Translation3d(qprime.X, qprime.Y, qprime.Z);
        }

        /// <summary>
        /// Adds two translations in 3D space and returns the sum.
        /// </summary>
        /// <param name="other">The translation to add.</param>
        /// <returns>The sum of the translations.</returns>
        public Translation3d Plus(Translation3d other)
        {
            return new Translation3d(X + other.X, Y + other.Y, Z + other.Z);
        }
        
        public Translation3d Minus(Translation3d other) {
            return new Translation3d(X - other.X, Y - other.Y, Z - other.Z);
        }

        /// <summary>
        /// Copies the data to a protobuf-sterilized version to be sent over NT
        /// </summary>
        /// <returns>The sterilized protobuf</returns>
        public ProtobufTranslation3d ToProtobuf()
        {
            var proto = new ProtobufTranslation3d
            {
                X = X,
                Y = Y,
                Z = Z,
            };
            return proto;
        }
        
        public override string ToString()
        {
            return $"Translation3d(X: {X}, Y: {Y}, Z: {Z})";
        }
    }
}
