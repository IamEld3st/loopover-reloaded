using System;
using UnityEngine;

public class BlocksManager : MonoBehaviour
{
    public int playfieldSize; // Size of playfield
    public GameObject blockPrefab; // Prefab of the block

    private BlockBehaviour[] _block; // Block array
    private float scaleRatio = 1.125f * 5; // Ratio for scale of playfield 
    
    // Start is called before the first frame update
    void Start()
    {   
        // Calculate and set the scale of playfield
        float actualScale = scaleRatio / playfieldSize;
        transform.localScale = new Vector3(actualScale, actualScale, actualScale);
        
        // Allocate block array
        _block = new BlockBehaviour[playfieldSize*playfieldSize];
        
        // Loop over each row
        for (int i = 0; i < playfieldSize; i++)
        {
            // Loop over all items in a row
            for (int j = 0; j < playfieldSize; j++)
            {
                // Create block
                GameObject block = Instantiate(blockPrefab, transform);
                
                // Calculate the position in array
                int pos = ((i * playfieldSize) + j);
                
                // Set block name
                block.name = "Block " + pos;
                
                // Find BlockBehaviour
                BlockBehaviour blockBehaviour = block.GetComponent<BlockBehaviour>();
                
                // Set block color
                float colorX = (float)(pos%playfieldSize)/(playfieldSize-1);
                float colorY = (pos/(float)playfieldSize)/(playfieldSize-1);
                blockBehaviour.bgColor = new Color((1-colorX)*1, colorY*1, colorX*1);
                
                // Find if playfield is even
                bool even = playfieldSize % 2 == 0;
                
                // Calculate block starting position
                float x = even ? (-(playfieldSize / 2f) + j) + 0.5f : Mathf.Ceil(-(playfieldSize / 2f) + j);
                float y = even ? (-(-(playfieldSize / 2f) + i)) - 0.5f : Mathf.Floor(-(-(playfieldSize / 2f) + i));
                
                // Move block to starting position
                block.transform.localPosition = new Vector3(x, y);
                
                // Set block target position (the same as start position)
                blockBehaviour.posX = x;
                blockBehaviour.posY = y;
                
                // Find block label
                // If playfield size is bigger than 5x5 use numbers.
                // If playfield size is smaller than 5x5 use letters.
                string displayLabel = playfieldSize > 5 ? ("" + (pos + 1)) : ("" + Char.ConvertFromUtf32(65 + pos));
                
                // Set block label
                blockBehaviour.label = displayLabel;
                
                // Setup reference in array
                _block[pos] = blockBehaviour;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check every block
        for (int i = 0; i < _block.Length; i++)
        {
            // Find out maximum distance from the center in both axis
            float odd = (playfieldSize % 2 == 1) ? 0f : 0.5f;
            float maxDist = Mathf.Floor(playfieldSize / (float) 2) - odd;
            
            if (!_block[i].outboundPermit) // If block is forbidden movement outside of play area
            {
                // Find out if block is outbound on any axis
                bool xOut = Mathf.Abs(_block[i].transform.localPosition.x) > maxDist + 0.00001f;
                bool yOut = Mathf.Abs(_block[i].transform.localPosition.y) > maxDist + 0.00001f;
                
                if (xOut || yOut)
                {
                    // Permit movement outside play area
                    _block[i].outboundPermit = true;
                    
                    // Create the ghost block
                    GameObject tempBlockGameObject = Instantiate(_block[i].gameObject, transform);
                    
                    // Find the outbound direction
                    float dirX = Mathf.Clamp(_block[i].transform.localPosition.x * 10f, -1f, 1f);
                    float dirY = Mathf.Clamp(_block[i].transform.localPosition.y * 10f, -1f, 1f);
                    
                    // Find out where to place the ghost block
                    float tempBlockX = xOut ? -dirX * (maxDist + 1f) : _block[i].transform.localPosition.x;
                    float tempBlockY = yOut ? -dirY * (maxDist + 1f) : _block[i].transform.localPosition.y;
                    
                    // Move the ghost block to the correct position
                    tempBlockGameObject.transform.localPosition = new Vector3(tempBlockX, tempBlockY);
                    
                    // Set reference to the ghost block in the original
                    _block[i].tempBlock = tempBlockGameObject.GetComponent<BlockBehaviour>();
                    
                    // Find out ghost block target position
                    float tempBlockPosX = xOut ? _block[i].posX + (-dirX * playfieldSize) : _block[i].posX;
                    float tempBlockPosY = yOut ? _block[i].posY + (-dirY * playfieldSize) : _block[i].posY;
                    
                    // Set ghost block target position
                    _block[i].tempBlock.posX = tempBlockPosX;
                    _block[i].tempBlock.posY = tempBlockPosY;
                }
            }
            else // If block is permitted movement outside play area
            {
                // Check if whole of original block is outbound
                if (_block[i].gridX > playfieldSize-1 || _block[i].gridX < 0 || _block[i].gridY > playfieldSize-1 || _block[i].gridY < 0)
                {
                    // Move it back to ghost block pos
                    _block[i].transform.localPosition = _block[i].tempBlock.gameObject.transform.localPosition;
                    
                    // Set same target as the ghost block
                    _block[i].posX = _block[i].tempBlock.posX;
                    _block[i].posY = _block[i].tempBlock.posY;
                    
                    // Destroy ghost block
                    Destroy(_block[i].tempBlock.gameObject);
                    
                    // Forbid free movement outside play area
                    _block[i].outboundPermit = false;
                }
            }
        }
    }
}
