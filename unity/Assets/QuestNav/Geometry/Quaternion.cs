using System;
using Newtonsoft.Json;
using Wpi.Proto;

namespace QuestNav.QuestNav. Geometry
{
    /// <summary>
    /// Represents a rotation in FRC 3d space
    /// </summary>
    public class Quaternion
    {
        [JsonProperty("X")]
        public double X { get; set; }
        [JsonProperty("Y")]
        public double Y { get; set; }
        [JsonProperty("Z")]
        public double Z { get; set; }
        [JsonProperty("W")]
        public double W { get; set; }
        
        /// <summary>
        /// Builds a new Quaternion with a default angle of 0 degrees. 
        /// </summary>
        public Quaternion()
        {
            W = 1.0;
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
        }
        
        /// <summary>
        /// Builds a new Quaternion
        /// </summary>
        /// <param name="w"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [JsonConstructor]
        public Quaternion(double w, double x, double y, double z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Builds a new Quaternion from its protobuf counterpart
        /// </summary>
        /// <param name="protobuf">The protobuf quaternion to copy</param>
        public Quaternion(ProtobufQuaternion protobuf)
        {
            X = protobuf. X;
            Y = protobuf. Y;
            Z = protobuf. Z;
            W = protobuf. W;
        }

        /// <summary>
        /// Multiply with another quaternion. 
        /// </summary>
        /// <param name="other">The other quaternion. </param>
        /// <returns>The quaternion product.</returns>
        public Quaternion Times(Quaternion other)
        {
            // https://en.wikipedia.org/wiki/Quaternion#Scalar_and_vector_parts
            var r1 = W;
            var r2 = other.W;

            // v₁ ⋅ v₂
            double dot = X * other.X + Y * other.Y + Z * other.Z;

            // v₁ x v₂
            double crossX = Y * other.Z - other.Y * Z;
            double crossY = other.X * Z - X * other.Z;
            double crossZ = X * other.Y - other.X * Y;

            return new Quaternion(
                // r = r₁r₂ − v₁ ⋅ v₂
                r1 * r2 - dot,
                // v = r₁v₂ + r₂v₁ + v₁ x v₂
                r1 * other.X + r2 * X + crossX,
                r1 * other. Y + r2 * Y + crossY,
                r1 * other. Z + r2 * Z + crossZ);
        }

        /// <summary>
        /// Returns the conjugate of the quaternion.
        /// </summary>
        /// <returns>The conjugate quaternion.</returns>
        public Quaternion Conjugate()
        {
            return new Quaternion(W, -X, -Y, -Z);
        }

        /// <summary>
        /// Returns the elementwise product of two quaternions. 
        /// </summary>
        /// <param name="other">The other quaternion. </param>
        /// <returns>The dot product of two quaternions.</returns>
        public double Dot(Quaternion other)
        {
            return W * other.W + X * other.X + Y * other. Y + Z * other.Z;
        }

        /// <summary>
        /// Calculates the L2 norm of the quaternion.
        /// </summary>
        /// <returns>The L2 norm. </returns>
        public double Norm()
        {
            return Math.Sqrt(Dot(this));
        }

        /// <summary>
        /// Divides by a scalar.
        /// </summary>
        /// <param name="scalar">The value to scale each component by.</param>
        /// <returns>The scaled quaternion.</returns>
        public Quaternion Divide(double scalar)
        {
            return new Quaternion(W / scalar, X / scalar, Y / scalar, Z / scalar);
        }

        /// <summary>
        /// Returns the inverse of the quaternion.
        /// </summary>
        /// <returns>The inverse quaternion.</returns>
        public Quaternion Inverse()
        {
            var norm = Norm();
            return Conjugate().Divide(norm * norm);
        }

        /// <summary>
        /// Converts the Quaternion to a ProtobufQuaternion
        /// </summary>
        /// <returns>A protobuf holding the quaternion</returns>
        public ProtobufQuaternion ToProtobuf()
        {
            var proto = new ProtobufQuaternion
            {
                X = X,
                Y = Y,
                Z = Z,
                W = W,
            };
            return proto;
        }
        
        public override string ToString()
        {
            return $"Quaternion(W:  {W}, X:  {X}, Y:  {Y}, Z:  {Z})";
        }
    }
}