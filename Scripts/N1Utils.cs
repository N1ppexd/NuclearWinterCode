using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace N1ppeUtils
{
    public static class NiilosExtensions
    {

        private static Camera camera;

        public static Camera Camera
        {
            get
            {
                if (camera == null) { camera = Camera.main; }
                return camera;
            }
        }


        /// <summary>
        /// Find the nearest object that has type <typeparamref name="T"/> to some position <paramref name="position"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <returns></returns>
        public static T FindNearest<T>(Vector3 position) where T : Component
        {
            T[] objects = Object.FindObjectsOfType<T>();
            T nearest = null;
            float minDistance = Mathf.Infinity;
            foreach (T obj in objects)
            {
                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = obj;
                }
            }
            return nearest;
        }


        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out WaitForSeconds wait)) return wait;

            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }




        //check if the mouse is over UI
        private static PointerEventData eventDataCurrentPosition;
        private static List<RaycastResult> results;

        public static bool IsOverUi()
        {
            eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }


        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
            return result;
        }


        /// <summary>
        /// Check if a <paramref name="renderer"/> is visible from a <paramref name="camera"/>
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }

        /// <summary>
        /// Delete all children of <paramref name="t"/>
        /// </summary>
        /// <param name="t"></param>
        public static void DeleteChildren(this Transform t)
        {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }


        /// <summary>
        /// Move children to another gameobject
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void MoveChildren(this GameObject from, GameObject to)
        {
            Transform[] children = new Transform[from.transform.childCount];
            for (int i = 0; i < from.transform.childCount; i++)
                children[i] = from.transform.GetChild(i);
            for (int i = 0; i < children.Length; i++)
                children[i].SetParent(to.transform);
        }


        /// <summary>
        /// Get random item from List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }


        /// <summary>
        /// Shuffle A list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = list.Count - 1; i > 1; i--)
            {
                var j = Random.Range(0, i + 1);
                var value = list[j];
                list[j] = list[i];
                list[i] = value;
            }
        }


        /// <summary>
        /// Change the direction of a rigidbody
        /// </summary>
        /// <param name="rb"></param>
        /// <param name="newDirection"></param>
        public static void ChangeDirection(this Rigidbody rb, Vector3 newDirection)
        {
            rb.velocity = newDirection.normalized * rb.velocity.magnitude;
        }





        public static bool IsInCameraViewPort(Camera camera, Vector3 target)
        {
            Vector3 v = camera.WorldToViewportPoint(target);
            return v.x > 0 && v.x < 1 && v.y > 0 && v.y < 1 && v.z > 0;
        }

        /// <summary>
        /// Check if a <paramref name="target"/> transform is visible in <paramref name="camera"/>. 
        /// Does not check if something obstructs the view.
        /// Returns either true or false..
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="target"></param>
        /// /// <returns> bool </returns>
        public static bool IsInCameraViewPort(Camera camera, Transform target)
        {
            return IsInCameraViewPort(camera, target.position);
        }


        /// <summary>
        /// Get the squared distance between transforms <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float SquaredDistance(Transform a, Transform b)
        {
            return (a.position - b.position).sqrMagnitude;
        }

        /// <summary>
        /// Is the path from <paramref name="pos1"/> to <paramref name="pos2"/> to blocked?
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static bool IsBlocked(Vector3 pos1, Vector3 pos2, LayerMask layerMask)
        {
            //Sends a raycast from pos 1 to pos2, anc seee if the path is blocked
            Vector3 dir = pos2 - pos1;


            return (Physics.Raycast(pos1, dir.normalized, out RaycastHit hit, dir.magnitude, layerMask));
        }


        /// <summary>
        /// Turn a layer to layermask
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static LayerMask LayerToMask(int layer) => LayerMask.GetMask(LayerMask.LayerToName(layer));

    }

    public static class NiilosArrayConversions
    {

        /// <summary>
        /// Get the hagged sizes from a jagged array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topArray"></param>
        /// <returns></returns>
        public static int[] JaggedSizes<T>(this T[][][] topArray)
        {
            List<int> rtn = new List<int>
            {
                //adds the top array length to the list
                topArray.Length
            };
            for (int i = 0; i < topArray.Length; i++)
            {
                var midArray = topArray[i];
                rtn.Add(midArray.Length);
                for (int j = 0; j < midArray.Length; j++)
                {
                    var botArray = midArray[j];
                    rtn.Add(botArray.Length);
                }
            }
            return rtn.ToArray();
        }




        /// <summary>
        /// Make a jagged array to a single dimensional array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jagged"></param>
        /// <returns></returns>
        public static T[] ToFlat<T>(this T[][][] jagged)
        {
            return jagged.SelectMany(inner => inner.SelectMany(innerInner => innerInner)).ToArray();
        }



        /// <summary>
        /// Convert a single dimensional array back into a jagged array by using the sizes of the jagged array, which should be known.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flat"></param>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public static T[][][] FromFlatWithSizes<T>(this T[] flat, int[] sizes)
        {
            int inPtr = 0;
            int sPtr = 0;
            int topSize = sizes[sPtr++];
            T[][][] rtn = new T[topSize][][];
            for (int i = 0; i < topSize; i++)
            {
                int midSize = sizes[sPtr++];
                T[][] mid = new T[midSize][];
                rtn[i] = mid;
                for (int j = 0; j < midSize; j++)
                {
                    int botSize = sizes[sPtr++];
                    T[] bot = new T[botSize];
                    mid[j] = bot;
                    for (int k = 0; k < botSize; k++)
                    {
                        bot[k] = flat[inPtr++];
                    }
                }
            }
            return rtn;
        }


        /// <summary>
        /// Swaps two parts of an array in indexes <paramref name="i"/> and <paramref name="i"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayObjects"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static T[] SwapParts<T>(T[] arrayObjects, int i, int j)
        {
            //swaps i to j
            (arrayObjects[i], arrayObjects[j]) = (arrayObjects[j], arrayObjects[i]);

            return arrayObjects;
        }
    }

    public abstract class NiilosMathStuff
    {

        /// <summary>
        /// Take the average of an array of vector2 <paramref name="vectors"/>
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static Vector2 Vector2Average(params Vector2[] vectors)
        {

            if (vectors.Length == 0)
                return Vector2.zero;

            float x = vectors.Average(v => v.x);
            float y = vectors.Average(v => v.y);

            return new Vector2(x, y);
        }


        /// <summary>
        /// Calculate the averag of an array of Vector3 <paramref name="vectors"/>
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static Vector3 Vector3Average(params Vector3[] vectors)
        {
            if (vectors.Length == 0)
                return Vector3.zero;

            float x = vectors.Average(v => v.x);
            float y = vectors.Average(v => v.y);
            float z = vectors.Average(v => v.z);

            return new Vector3(x, y, z);
        }


        /// <summary>
        /// Direction from a vector3 <paramref name="from"/> to a vector3 <paramref name="to"/>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector3 Direction(Vector3 from, Vector3 to) { return (to - from).normalized; }



        /// <summary>
        /// Transforms a matrix4x4 into a rotation.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Quaternion MatrixToRotation(Matrix4x4 m)
        {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Quaternion q = new Quaternion
            {
                w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2,
                x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2,
                y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2,
                z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2
            };
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
        }


        /// <summary>
        /// Calculate the Quadratic Beizier based on <paramref name="t"/>, <paramref name="P0"/>, <paramref name="P1"/> and <paramref name="P2"/>
        /// </summary>
        /// <param name="t"></param>
        /// <param name="P0"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        public static Vector3 QuadraticBezier(float t, Vector3 P0, Vector3 P1, Vector3 P2)
        {
            float u = 1 - t;
            return (u * u * P0) + (2 * u * t * P1) + (t * t * P2);
        }


        /// <summary>
        /// Calulate the factorial of <paramref name="n"/>
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Factorial(int n)
        {
            //check werther n is less than 0
            if (n < 0) { Debug.LogError("Factorial cannot be done for negative numbers"); return 0; }

            int result = 1;
            for (int i = n; i > 0; i--)
            {
                result *= i;
            }

            return result;
        }


        //A function to calculate the factorial of a number
        public static int FactorialRecursive(int n)
        {
            if (n == 0) return 1;
            return n * FactorialRecursive(n - 1);
        }



        /// <summary>
        /// Calculates the Binomial Coefficient of <paramref name="n"/> and <paramref name="k"/>
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int BinomialCoefficient(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }



        /// <summary>
        /// calculates the binomial coefficient recursively
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int BinomialCoefficientRecursive(int n, int k)
        {
            if (k == 0 || k == n) return 1;
            return BinomialCoefficientRecursive(n - 1, k - 1) + BinomialCoefficientRecursive(n - 1, k);
        }

        #region Bionimial Coefficcient functions

        private static Dictionary<(int, int), int> memo = new Dictionary<(int, int), int>();
        /// <summary>
        /// Calculates the binomial coefficient recursively with memoization
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int BinomialCoefficientMemo(int n, int k)
        {
            if (k == 0 || k == n) return 1;
            if (memo.ContainsKey((n, k))) return memo[(n, k)];
            int result = BinomialCoefficientMemo(n - 1, k - 1) + BinomialCoefficientMemo(n - 1, k);
            memo[(n, k)] = result;
            return result;
        }


        // a fast way to calculate the binomial coefficient
        public static int BinomialCoefficientFast(int n, int k)
        {
            if (k == 0 || k == n) return 1;
            if (k == 1 || k == n - 1) return n;
            if (k > n / 2) k = n - k;
            int result = n;
            for (int i = 2; i <= k; i++)
            {
                result *= (n - i + 1);
                result /= i;
            }
            return result;
        }


        /// <summary>
        /// function that calculates the binomial coefficient using pascals triangle
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int BinomialCoefficientPascal(int n, int k)
        {
            int[][] pascal = new int[n + 1][];
            for (int i = 0; i <= n; i++)
            {
                pascal[i] = new int[i + 1];
                pascal[i][0] = 1;
                pascal[i][i] = 1;
                for (int j = 1; j < i; j++)
                {
                    pascal[i][j] = pascal[i - 1][j - 1] + pascal[i - 1][j];
                }
            }
            return pascal[n][k];
        }

        #endregion


        //a fast way to verify if a number is prime
        public static bool IsPrime(int n)
        {
            if (n == 2) return true;
            if (n % 2 == 0) return false;
            int sqrt = (int)Mathf.Sqrt(n);
            for (int i = 3; i <= sqrt; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        /// <summary>
        /// The greatest common divisor of two numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GCD(int a, int b)
        {
            if (b == 0) return a;
            return GCD(b, a % b);
        }


        //a function to calculate the least common multiple of two numbers
        public static int LCM(int a, int b)
        {
            return (a * b) / GCD(a, b);
        }


        /// <summary>
        /// See if float <paramref name="a"/> is between <paramref name="b"/> and <paramref name="c"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool isBetween(float a, float b, float c)
        {
            return a <= b && b <= c;
        }
    }
}
