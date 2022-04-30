using Assets.Scripts.Player;
using Assets.Utilities;
using System;
using UnityEngine;


public class CameraRig : MonoBehaviour
{
    [field: SerializeField]
    public bool DebugCamera { get; set; } = true;

    [field: Header("Follow")]

    [SerializeField, RequireInterface(typeof(IPlayer))]
    private UnityEngine.Object _player;
    public IPlayer Player => _player as IPlayer;

    [field: SerializeField]
    public float CamDistsAdaptSpeed { get; set; } = 3f;
    public float[] CamDists = new float[4];

    [field: SerializeField]
    public int ZoomStages { get; set; } = 10;


    [field: Header("Pivoting And Looking")]

    [field: SerializeField]
    public float FollowSpeedsAdaptSpeed { get; set; } = 6;
    public float[] FollowSpeeds = new float[4];


    [field: SerializeField]
    public float RotationSpeedsAdaptSpeed { get; set; } = 3;

    public float[] RotationSpeeds = new float[4];


    [field: SerializeField]
    public float CamPivotSpeed { get; set; } = .00005f;

    [field: SerializeField]
    public float CamPivotOffsetFromPlayerY { get; set; } = 1.5f;

    [field: SerializeField]
    public Vector2 PivotLimit { get; set; }


    [field: Header("Left & Right Compensation")]

    [field: SerializeField]
    public Vector2 LookLRPositionCompensatorLimits { get; set; }

    public float[] LookLRPositionCompensatorSpeeds = new float[4];

    public float[] ViewportDesiredLROffsetFromCenter = new float[4];


    [field: Header("Collision")]

    [field: SerializeField]
    public int BufferSize { get; set; } = 4;

    [field: SerializeField]
    public LayerMask AgainstLayers { get; set; }

    [field: SerializeField]
    public float CameraSPhereRadius { get; set; } = 0.2f;

    [field: SerializeField]
    public float ClosenessConstraint { get; set; } = 0.2f;

    private float _pivotYAdditive;
    private float _zoomPerc = 1f;

    private void Start()
    {
        if (LookLRPositionCompensatorSpeeds == null || LookLRPositionCompensatorSpeeds.Length < 4)
        {
            throw new Exception("Set 4 different speeds for LookLRPositionCompensatorSpeeds please!");
        }

        Player.Brain.OnLook.AddListener(v => { _pivotYAdditive = Mathf.Clamp(_pivotYAdditive - v.y * CamPivotSpeed, PivotLimit.x, PivotLimit.y); });
        Player.Brain.OnZoom.AddListener(f => _zoomPerc = Mathf.Clamp01(_zoomPerc + (f / ZoomStages)));
        _buff = new RaycastHit[BufferSize];
    }

    private Vector3 _velSmoothDampDump;
    private float _rotationSpeed;
    private float _followSpeed;
    private float _camDist;
    private float _dampenedOrientation;
    private void LateUpdate()
    {
        _dampenedOrientation = Mathf.Lerp(_dampenedOrientation, Player.Orientation, LookLRPositionCompensatorSpeeds[Player.Movement] * Time.deltaTime);
        _rotationSpeed = Mathf.Lerp(_rotationSpeed, RotationSpeeds[Player.Movement], RotationSpeedsAdaptSpeed * Time.deltaTime);
        _followSpeed = Mathf.Lerp(_followSpeed, FollowSpeeds[Player.Movement], FollowSpeedsAdaptSpeed * Time.deltaTime);
        _camDist = Mathf.Lerp(_camDist, CamDists[Player.Movement], CamDistsAdaptSpeed * Time.deltaTime);

        var pivotOffset = new Vector3(0, _pivotYAdditive + CamPivotOffsetFromPlayerY, 0);
        var camDist = Mathf.Max(GetCollisionAdjustedDis(Player.Transform.position + pivotOffset, _zoomPerc, _camDist), ClosenessConstraint);

        //project from screen desired coords to world space the translate difference
        //that would place the camera in the ideal spot towards the player
        var translateDiff = Player.Transform.position +
            pivotOffset -//take pivot and offset into consideration
            Camera.main.ViewportToWorldPoint(new Vector3(
                Mathf.Lerp(LRViewportPos(1), LRViewportPos(-1), (_dampenedOrientation + 1) / 2), //desired pos on screen based on the rule of thirds
                0.5f, //middle of the screen
                camDist));
        transform.position = Vector3.SmoothDamp(transform.position, transform.position + translateDiff , ref _velSmoothDampDump, _followSpeed * Time.deltaTime);
        
        //--rotation
        var desiredCamDir = (Player.Transform.position + Player.Transform.forward * camDist - new Vector3(0, _pivotYAdditive, 0) - Player.Transform.position).normalized;
        transform.forward += (desiredCamDir - transform.forward) * Mathf.Clamp((transform.forward - desiredCamDir).magnitude, 0, Time.deltaTime * _rotationSpeed);
    }
    private RaycastHit[] _buff;
    private float _closestGroundHitDist;
    private float GetCollisionAdjustedDis(Vector3 playerPos, float distPercentage, float idealCamDist)
    {
        _closestGroundHitDist = idealCamDist;

        var buffCount = Physics.SphereCastNonAlloc(playerPos, CameraSPhereRadius, (transform.position - playerPos).normalized, _buff, idealCamDist, AgainstLayers);

        for (byte i = 0; i < buffCount; i++)
        {
            if (_closestGroundHitDist > _buff[i].distance)
            {
                _closestGroundHitDist = _buff[i].distance;
            }
        }

        return _closestGroundHitDist * distPercentage;
    }

    private float LRViewportPos(int dir)
    {
        return 0.5f + ViewportDesiredLROffsetFromCenter[Player.Movement] * dir;
    }
}
