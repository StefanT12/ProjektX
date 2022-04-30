using UnityEngine;

public class ManualSimulation : MonoBehaviour
{
    private float _timer;
    [field: SerializeField]
    public bool AutoSimulate { get; set; } = true;

    private void Awake()
    {
        Physics.autoSimulation = AutoSimulate;
    }

    void Update()
    {
        if (Physics.autoSimulation)
            return; // do nothing if the automatic simulation is enabled

        _timer += Time.deltaTime;

        // Catch up with the game time.
        // Advance the physics simulation in portions of Time.fixedDeltaTime
        // Note that generally, we don't want to pass variable delta to Simulate as that leads to unstable results.
        while (_timer >= Time.deltaTime)
        {
            _timer -= Time.deltaTime;
            Physics.Simulate(Time.deltaTime);
        }

        // Here you can access the transforms state right after the simulation, if needed
    }
}