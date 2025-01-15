using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PuggleAgent : Agent
{

    public LevelGeneration levelGeneration;
    public int maxObservationNb;

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        Vector3 Dir = Vector3.zero;

        Dir.x = Mathf.Clamp(actions.ContinuousActions[0], -1, 1);
        Dir.y = Mathf.Clamp(actions.ContinuousActions[1], -1, 0);

        levelGeneration.Shoot(Dir);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        if (levelGeneration.marbles.Count/3f > maxObservationNb)
        {
            Debug.LogWarning("Trop de..");
            return;
        }

        foreach (var item in levelGeneration.marbles)
        {
            if (item == null)
            {
                sensor.AddObservation(-10000);
                sensor.AddObservation(-10000);
                sensor.AddObservation(0);
            }
            else
            {
                sensor.AddObservation(item.transform.position.x);
                sensor.AddObservation(item.transform.position.y);

                switch (item.tag)
                {
                    case "Blue":
                        sensor.AddObservation(1);
                        break;
                    case "Orange":
                        sensor.AddObservation(2);
                        break;
                    case "Green":
                        sensor.AddObservation(3);
                        break;
                    default:
                        break;
                }
            }
        }

        for (int i = (levelGeneration.marbles.Count*3); i < maxObservationNb; i++)
        {
            sensor.AddObservation(10000);
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
