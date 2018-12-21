using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    public float smoothTime = 4f;
    public float posX;
    public float posY;
    public int gridX;
    public int gridY;
    public string label;
    public Color bgColor = Color.white;
    public bool even;
    public bool outboundPermit;
    public BlockBehaviour tempBlock;
    
    private SpriteRenderer _background;
    private TextMesh _label;

    private Vector3 _target;
    //private Vector3 _velocity = Vector3.zero;
    private bool _movement;
    private bool _lockX, _lockY;
    private BlocksManager _manager;

    void Start()
    {
        _background = GetComponentInChildren<SpriteRenderer>();
        _label = GetComponentInChildren<TextMesh>();
        _manager = GetComponentInParent<BlocksManager>();
    }

    void Update()
    {
        _target = new Vector3(posX, posY);
        float distance = Vector3.Distance(transform.localPosition, _target);
        float step = distance * Time.deltaTime * smoothTime;
        if (step < smoothTime/500) step = smoothTime/500;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, _target, step);
        if (distance < 0.0001f)
        {
            _movement = false;
            transform.localPosition = _target;
            float odd = (_manager.playfieldSize % 2 == 1) ? 0f : 0.5f;
            float maxDist = Mathf.Floor(_manager.playfieldSize / (float) 2) - odd;
            gridX = (int)(maxDist + posX);
            gridY = _manager.playfieldSize - 1 - (int)(maxDist + posY);
        }
        //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _target, ref _velocity, smoothTime);
        _background.color = bgColor;
        _label.text = label;
    }

    void OnMouseDrag()
    {
        if (!_movement)
        {
            _lockX = false;
            _lockY = false;

        }
        Vector3 dragPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 dragRelativeToBlockGrid = transform.parent.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(dragPos));
        float xCursor = dragRelativeToBlockGrid.x;
        float yCursor = dragRelativeToBlockGrid.y;
        float xDist = Mathf.Abs(xCursor - transform.localPosition.x);
        float yDist = Mathf.Abs(yCursor - transform.localPosition.y);
        //Debug.Log(xDist + " " + yDist);
        if (even)
        {
            if (xDist > 0.5f && !_lockX) // X Axis
            {
                int tempX = (int)xCursor;
                if (xCursor - tempX >= 0.5f) posX = tempX + 0.5f;
                else posX = tempX - 0.5f;
                _lockY = true;
            }
            else if (yDist > 0.5f && !_lockY) // Y Axis
            {
                int tempY = (int)yCursor;
                if (yCursor - tempY >= 0.5f) posY = tempY + 0.5f;
                else posY = tempY - 0.5f;
                _lockX = true;
            }
        }
        else
        {
            if (xDist > 0.5f && !_lockX) // X Axis
            {
                posX = Mathf.Round(xCursor);
                _movement = true;
                _lockY = true;
            }
            else if (yDist > 0.5f && !_lockY) // Y Axis
            {
                posY = Mathf.Round(yCursor);
                _movement = true;
                _lockX = true;
            }
        }
    }
}
