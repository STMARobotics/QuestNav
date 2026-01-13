namespace QuestNav.QuestNav.Geometry
{
    public class Transform3d
    {
        public Translation3d Translation { get; }
        public Rotation3d Rotation { get; }

        public Transform3d(Translation3d translation, Rotation3d rotation)
        {
            Translation = translation;
            Rotation = rotation;
        }
        
        public Transform3d(Pose3d initial, Pose3d last) {
            // We are rotating the difference between the translations
            // using a clockwise rotation matrix. This transforms the global
            // delta into a local delta (relative to the initial pose).
            Translation =
                last.Translation
                    .Minus(initial.Translation)
                    .RotateBy(initial.Rotation.UnaryMinus());

            Rotation = last.Rotation.Minus(initial.Rotation);
        }
        
        public Transform3d Plus(Transform3d other) {
            return new Transform3d(Pose3d.Zero, Pose3d.Zero.TransformBy(this).TransformBy(other));
        }
    }
}
