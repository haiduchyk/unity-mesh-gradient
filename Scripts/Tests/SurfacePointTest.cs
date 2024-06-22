namespace Pandora.MeshGradient
{
    using System.Collections.Generic;
    using UnityEngine;

    // made by ai
    public class SurfacePointTest : MonoBehaviour
    {
        void Start()
        {
            RunTests();
        }

        void RunTests()
        {
            var testCases = GetTestCases();
            foreach (var testCase in testCases)
            {
                RunTest(testCase);
            }
        }

        List<TestCase> GetTestCases()
        {
            return new List<TestCase>
            {
                new TestCase(
                    0.17272733125634054f,
                    0.8668437702237672f,
                    new Matrix4x4(
                        new Vector4(0.7460012f, 0.77237373f, 0.99832747f, 0.49966165f),
                        new Vector4(0.63355551f, 0.90940385f, 0.65119572f, 0.20029142f),
                        new Vector4(0.90449301f, 0.27186736f, 0.07489489f, 0.26654596f),
                        new Vector4(0.55285245f, 0.94662943f, 0.02056269f, 0.89588302f)
                    ),
                    new Matrix4x4(
                        new Vector4(0.22827844f, 0.23082185f, 0.39406971f, 0.72516512f),
                        new Vector4(0.16500692f, 0.34764502f, 0.70261069f, 0.71364357f),
                        new Vector4(0.61725326f, 0.00226673f, 0.89220507f, 0.96308519f),
                        new Vector4(0.79884797f, 0.32920004f, 0.07102013f, 0.38117077f)
                    ),
                    new Vector2(0.7603472f, 0.15947474f)
                ),
                new TestCase(
                    0.0027526014932683918f,
                    0.45588041628068166f,
                    new Matrix4x4(
                        new Vector4(0.7460012f, 0.77237373f, 0.99832747f, 0.49966165f),
                        new Vector4(0.63355551f, 0.90940385f, 0.65119572f, 0.20029142f),
                        new Vector4(0.90449301f, 0.27186736f, 0.07489489f, 0.26654596f),
                        new Vector4(0.55285245f, 0.94662943f, 0.02056269f, 0.89588302f)
                    ),
                    new Matrix4x4(
                        new Vector4(0.22827844f, 0.23082185f, 0.39406971f, 0.72516512f),
                        new Vector4(0.16500692f, 0.34764502f, 0.70261069f, 0.71364357f),
                        new Vector4(0.61725326f, 0.00226673f, 0.89220507f, 0.96308519f),
                        new Vector4(0.79884797f, 0.32920004f, 0.07102013f, 0.38117077f)
                    ),
                    new Vector2(0.83735307f, 0.20155417f)
                ),
                new TestCase(
                    0.1023006395414574f,
                    0.461362667096333f,
                    new Matrix4x4(
                        new Vector4(0.7460012f, 0.77237373f, 0.99832747f, 0.49966165f),
                        new Vector4(0.63355551f, 0.90940385f, 0.65119572f, 0.20029142f),
                        new Vector4(0.90449301f, 0.27186736f, 0.07489489f, 0.26654596f),
                        new Vector4(0.55285245f, 0.94662943f, 0.02056269f, 0.89588302f)
                    ),
                    new Matrix4x4(
                        new Vector4(0.22827844f, 0.23082185f, 0.39406971f, 0.72516512f),
                        new Vector4(0.16500692f, 0.34764502f, 0.70261069f, 0.71364357f),
                        new Vector4(0.61725326f, 0.00226673f, 0.89220507f, 0.96308519f),
                        new Vector4(0.79884797f, 0.32920004f, 0.07102013f, 0.38117077f)
                    ),
                    new Vector2(0.87751146f, 0.22471413f)
                ),
                new TestCase(
                    0.33802544487365305f,
                    0.9987107919506583f,
                    new Matrix4x4(
                        new Vector4(0.7460012f, 0.77237373f, 0.99832747f, 0.49966165f),
                        new Vector4(0.63355551f, 0.90940385f, 0.65119572f, 0.20029142f),
                        new Vector4(0.90449301f, 0.27186736f, 0.07489489f, 0.26654596f),
                        new Vector4(0.55285245f, 0.94662943f, 0.02056269f, 0.89588302f)
                    ),
                    new Matrix4x4(
                        new Vector4(0.22827844f, 0.23082185f, 0.39406971f, 0.72516512f),
                        new Vector4(0.16500692f, 0.34764502f, 0.70261069f, 0.71364357f),
                        new Vector4(0.61725326f, 0.00226673f, 0.89220507f, 0.96308519f),
                        new Vector4(0.79884797f, 0.32920004f, 0.07102013f, 0.38117077f)
                    ),
                    new Vector2(0.7769276f, 0.23620459f)
                ),
                new TestCase(
                    0.004244695747771421f,
                    0.7480709361371632f,
                    new Matrix4x4(
                        new Vector4(0.7460012f, 0.77237373f, 0.99832747f, 0.49966165f),
                        new Vector4(0.63355551f, 0.90940385f, 0.65119572f, 0.20029142f),
                        new Vector4(0.90449301f, 0.27186736f, 0.07489489f, 0.26654596f),
                        new Vector4(0.55285245f, 0.94662943f, 0.02056269f, 0.89588302f)
                    ),
                    new Matrix4x4(
                        new Vector4(0.22827844f, 0.23082185f, 0.39406971f, 0.72516512f),
                        new Vector4(0.16500692f, 0.34764502f, 0.70261069f, 0.71364357f),
                        new Vector4(0.61725326f, 0.00226673f, 0.89220507f, 0.96308519f),
                        new Vector4(0.79884797f, 0.32920004f, 0.07102013f, 0.38117077f)
                    ),
                    new Vector2(0.74656753f, 0.14691931f)
                )
            };
        }

        void RunTest(TestCase testCase)
        {
            var result = MeshGradientStaticEffect.SurfacePoint(testCase.u, testCase.v, testCase.X, testCase.Y);

            if (Vector2.Distance(result, testCase.expectedPoint) < 0.001f)
            {
                Debug.Log("Test passed!");
            }
            else
            {
                Debug.LogError("Test failed. Expected: " + testCase.expectedPoint + " but got: " + result);
            }
        }


        class TestCase
        {
            public float u;
            public float v;
            public Matrix4x4 X;
            public Matrix4x4 Y;
            public Vector2 expectedPoint;

            public TestCase(float u, float v, Matrix4x4 X, Matrix4x4 Y, Vector2 expectedPoint)
            {
                this.u = u;
                this.v = v;
                this.X = X;
                this.Y = Y;
                this.expectedPoint = expectedPoint;
            }
        }
    }
}