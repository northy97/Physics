using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Sphere
{
    public float Radius;
    public float Mass;
    public float bounciness;

}
public class physics : MonoBehaviour
{
    // Start is called before the first frame update
    public physics RigidBodyTemp;
    public Vector3 initialVelocity;
    public Vector3 LinearVelocity;
    public Vector3 linearVelocityinit;

    Sphere Ball;

    const float GravitationalConstant = 9.81f;


    // Start is called before the first frame update


    void Start()
    {

        initialVelocity = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f));
        LinearVelocity = initialVelocity;
        Ball.Radius = 0.5f;
        Ball.Mass = 1f;
        Ball.bounciness = 0.43f;

    }
    bool collisionCheckSphere(GameObject sphere)
    {
        Vector3 sphereNormal = sphere.transform.TransformDirection(0, 0, 0);
        Vector3 distance = sphere.transform.position - transform.position;

        float distanceSq = Vector3.Dot(distance, distance);
        float radiiSumSq = Ball.Radius + Ball.Radius;// currently because all spheres are the same
        radiiSumSq *= radiiSumSq;

        if (distanceSq <= radiiSumSq)
        {
            //transform.position += sphereNormal * (radiiSumSq - distanceSq);
            return true;
        }
        return false;
    }


    bool collisionCheckPlane(GameObject plane)// collision check plane
    {

        Vector3 normal = plane.transform.TransformDirection(Vector3.up);
        float distance = Vector3.Dot((transform.position - plane.transform.position), normal);
        if (distance <= Ball.Radius)
        {
            transform.position += normal * (Ball.Radius - distance);
            return true;
        }
        return false;
    }

    Vector3 applyForces()
    {
        return new Vector3(0, Ball.Mass * -9.81f, 0);
    }

    // Update is called once per frame
    void Update()
    {

        Object[] planes = GameObject.FindGameObjectsWithTag("plane");
        Object[] spheres = GameObject.FindGameObjectsWithTag("sphere");

        // use delta time to set things to more appropriate speed
        linearVelocityinit = LinearVelocity;


        Vector3 Force = applyForces();
        Vector3 LinearAcceleration = new Vector3(Force.x / Ball.Mass, Force.y / Ball.Mass, Force.z / Ball.Mass);
        LinearVelocity += LinearAcceleration * Time.deltaTime;

        transform.Translate(LinearVelocity * Time.deltaTime);




        List<GameObject> planeHit = new List<GameObject>();
        foreach (GameObject plane in planes)
        {

            if (collisionCheckPlane(plane))
            {


                planeHit.Add(plane);
                //  LinearVelocity = new Vector3(0, 0, 0);
                float totalRestitution = 0.92f;
                LinearVelocity = linearVelocityinit * totalRestitution; // fuged t

                for (int x = 0; x < planeHit.Count; x++)
                {
                    LinearVelocity = Vector3.Reflect(LinearVelocity, planeHit[x].transform.TransformDirection(Vector3.up));

                }

            }

        }

        List<GameObject> sphereHit = new List<GameObject>();

        foreach (GameObject sphere in spheres)
        {

            if (collisionCheckSphere(sphere) && name != sphere.name)
            {
                sphereHit.Add(sphere);
                Vector3 nv1;
                Vector3 nv2;
                Vector3 tempPos = transform.position;
                float A1, A2;
                float p;
                for (int x = 0; x < sphereHit.Count; x++)
                {
                    RigidBodyTemp = sphereHit[x].GetComponent<physics>();
                    Vector3 vectorBetween = sphere.transform.position - transform.position;
                    Vector3.Normalize(vectorBetween);
                    A1 = Vector3.Dot(LinearVelocity, vectorBetween);
                    A2 = Vector3.Dot(RigidBodyTemp.LinearVelocity, vectorBetween);




                    nv1 = LinearVelocity;
                    nv1 += Vector3.Project(RigidBodyTemp.LinearVelocity, (sphereHit[x].transform.position - transform.position));
                    nv1 -= Vector3.Project(LinearVelocity, (transform.position - sphereHit[x].transform.position));
                    nv2 = RigidBodyTemp.LinearVelocity;
                    nv2 += Vector3.Project(LinearVelocity, (sphereHit[x].transform.position - transform.position));
                    nv2 -= Vector3.Project(RigidBodyTemp.LinearVelocity, (transform.position - sphereHit[x].transform.position));

                    p = (Ball.bounciness + RigidBodyTemp.Ball.bounciness) * (A1 - A2) / (Ball.Mass + RigidBodyTemp.Ball.Mass);


                    LinearVelocity = nv1 - p * Ball.Mass * vectorBetween;
                    RigidBodyTemp.LinearVelocity = nv2 + p * Ball.Mass * vectorBetween;

                }



            }
        }







    }

}