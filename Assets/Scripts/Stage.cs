using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    //�ʿ� �ҽ� �ҷ�����
    [Header("Source")]
    public GameObject tilePrefab; // Ÿ�� ������ ��������
    public Transform backgroundNode;
    public Transform boardNode;
    public Transform tetrominoNode;
    public Transform previewNode;
    public GameObject gameoverPanel;
    public GameObject StartingCover;
    public Text score;
    public Text level;
    public Text line;

    private int indexVal = -1;
    //inspector��ǥ��Ǵ� ������ ����


    [Header("Setting")]
    [Range(4, 40)]
    public int boardWidth = 10;
    //���� ����
    [Range(5, 20)]
    public int boardHeight = 20;
    //�������� �ӵ�
    public float fallCycle = 1.0f;
    //��ġ ����
    public float offset_x = 0f;
    public float offset_y = 0f;

    private int halfWidth;
    private int halfHeight;

    private float nextFallTime; // ������ ��Ʈ�ι̳밡 ������ �ð��� ����

    // UI ���� ����
    private int scoreVal = 0;
    private int levelVal = 1;
    private int lineVal;


    private void Start() // ������ ���۵ǰ� �� ���� ����
    {
        // ���� ���۽� text ����
        lineVal = levelVal * 2;   // �ӽ� ���� ������
        score.text = "" + scoreVal;
        level.text = "" + levelVal;
        line.text = "" + lineVal;
        gameoverPanel.SetActive(false);
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);    // �ʺ��� �߰��� �������ֱ�
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);   // ������ �߰��� �������ֱ�

        nextFallTime = Time.time + fallCycle;   // ������ ��Ʈ�ι̳밡 ������ �ð� ����

        CreateBackground(); // ��� �����
        // ���̸�ŭ �� ��� ������ֱ�
        for (int i = 0; i < boardHeight; ++i)
        {
            // ToString�� �̿��Ͽ� ������Ʈ �̸� ����
            var col = new GameObject((boardHeight - i - 1).ToString());
            // ��ġ���� -> �� ��ġ�� ����, ���� �߾�
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            // ���� ����� �ڽ����� ����
            col.transform.parent = boardNode;
        }
        CreateTetromino(); // ��Ʈ�ι̳� �����
    }

    public bool longtouch_check = false;
    public bool lnt_enter = false;
    public bool leftbtn = false;
    public bool rightbtn = false;
    public bool downbtn = false;
    public bool rotbtn = false;
    public bool restart = false;
    private float touchtime = 0;
    private float time_check = 0.8f;

    public void longinf()
    {
        Debug.Log("longin");
        longtouch_check = true;
    }
    public void longout()
    {
        Debug.Log("longout");
        longtouch_check = false;
        touchtime = 0;
    }
    public void longtouch_checkf()
    {   
    }
    public void leftbtnf()
    {
        leftbtn = true;
    }
    public void rightbtnf()
    {
        rightbtn = true;
    }
    public void downbtnf()
    {
        downbtn = true;
    }
    public void rotbtnf()
    {
        rotbtn = true;
    }
    public void btn_restart() {
        restart = true;
    }

    void Update()   // �� �����Ӹ��� ����
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
        // ���ӿ��� ó��
        if (gameoverPanel.activeSelf)
        {
            if (Input.GetKeyDown("r")||restart)
            {
                restart = false;
                SceneManager.LoadScene(0);
            }
        }
        // ���� ó��
        else
        {
            //�ʱ�ȭ
            Vector3 moveDir = Vector3.zero; //�̵� ���� �����
            bool isRotate = false;  // ȸ�� ���� �����

            //�� Ű�� ���� �̵� ���� Ȥ�� ȸ�� ���θ� �������ݴϴ�.

            if (Input.GetKeyDown("a")||leftbtn)
            {
                moveDir.x = -1;
                leftbtn = false;

            }
            else if (Input.GetKeyDown("d")||rightbtn)
            {
                moveDir.x = 1;
                rightbtn = false;
            }

            if (Input.GetKeyDown("w")||rotbtn)
            {
                isRotate = true;
                rotbtn = false;
            }
            else if (Input.GetKeyDown("s")||downbtn)
            {
                Debug.Log("keydown");
                moveDir.y = -1;
                downbtn = false;
            }

            if (Input.GetKeyDown("space"))
            {
                MoveTetromino(Vector3.down, false);
            }
            else if(longtouch_check){
                touchtime += Time.deltaTime;
                Debug.Log(touchtime);
                if(touchtime >= time_check)
                {
                    while (MoveTetromino(Vector3.down, false))
                    {
                    }
                    touchtime = 0;
                    longtouch_check = false;
                }
            }

            if (Input.GetKeyDown("r")||restart)
            {
                // SceneManager�� �̿��Ͽ� ���� ������ϱ�
                restart = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }


            // �ƹ��� Ű �Է��� ������� Tetromino �������� �ʰ��ϱ�
            if (moveDir != Vector3.zero || isRotate)
            {
                MoveTetromino(moveDir, isRotate);
            }
            // �Ʒ��� �������� ���� ������ �̵���ŵ�ϴ�.
            if (Time.time > nextFallTime)
            {
                nextFallTime = Time.time + fallCycle;   // ���� ������ �ð� �缳��
                moveDir.y = -1; //  �Ʒ��� �� ĭ �̵�
                isRotate = false;   // ������ �̵��� ȸ�� ����

                // �ƹ��� Ű �Է��� ������� Tetromino �������� �ʰ��ϱ�
                if (moveDir != Vector3.zero || isRotate)
                {
                    MoveTetromino(moveDir, isRotate);
                }
            }
        }
    }

    // �̵��� �������� üũ�� True or False ��ȯ�ϴ� �޼���
    bool MoveTetromino(Vector3 moveDir, bool isRotate)
    {
        //�̵� or ȸ�� �Ұ��� ���ư��� ���� �� ����
        Vector3 oldPos = tetrominoNode.transform.position;
        Quaternion oldRot = tetrominoNode.transform.rotation;

        //�̵� & ȸ�� �ϱ�
        tetrominoNode.transform.position += moveDir;
        if (isRotate)
        {
            //���� ��Ʈ�ι̳� ��忡 90�� ȸ���� ���� ��.
            tetrominoNode.transform.rotation *= Quaternion.Euler(0, 0, 90);
        }

        // �̵� �Ұ��� ���� ��ġ, ȸ�� ���� ���ư���
        if (!CanMoveTo(tetrominoNode))
        {
            tetrominoNode.transform.position = oldPos;
            tetrominoNode.transform.rotation = oldRot;

            //�ٴڿ� ��Ҵٴ� �ǹ� = �̵� �Ұ��ϰ� ���� �Ʒ��� �������� �ִ� ��Ȳ
            if ((int)moveDir.y == -1 && (int)moveDir.x == 0 && isRotate == false)
            {
                AddToBoard(tetrominoNode);
                CheckBoardColumn();
                CreateTetromino();

                //��Ʈ�ι̳� ���� �߰� ���� �̵� ���� Ȯ��
                if (!CanMoveTo(tetrominoNode))
                {
                    gameoverPanel.SetActive(true);
                }
            }

            return false;
        }

        return true;
    }

    // ��Ʈ�ι̳븦 ���忡 �߰�
    void AddToBoard(Transform root)
    {
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);

            //����Ƽ ��ǥ�迡�� ��Ʈ���� ��ǥ��� ��ȯ
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            //�θ��� : �� ���(y ��ġ), ������Ʈ �̸� : x ��ġ
            node.parent = boardNode.Find(y.ToString());
            node.name = x.ToString();
        }
    }

    // ���忡 �ϼ��� ���� ������ ����
    void CheckBoardColumn()
    {
        bool isCleared = false;

        //�ѹ��� ����� �� ���� Ȯ�ο�
        int linecount = 0;

        // �ϼ��� �� == ���� �ڽ� ������ ���� ũ��
        foreach (Transform column in boardNode)
        {
            if (column.childCount == boardWidth)
            {
                //���� ��� �ڽ��� ����
                foreach (Transform tile in column)
                {
                    Destroy(tile.gameObject);
                }
                // ���� ��� �ڽĵ���� ���� ����
                column.DetachChildren();
                isCleared = true;
                linecount++; // �ϼ��� �� �ϳ��� linecount ����
            }
        }
        // �ϼ��� ���� ������� ��������
        if (linecount != 0)
        {
            scoreVal += linecount * linecount * 100;
            score.text = "" + scoreVal;
        }

        // �ϼ��� ���� ������� �������� ����
        if (linecount != 0)
        {
            lineVal -= linecount;
            // ���������� �ʿ� ���� ���ް�� (�ִ� ���� 10���� ����)
            if (lineVal <= 0 && levelVal <= 10)
            {
                levelVal += 1;  // ��������
                lineVal = levelVal * 2 + lineVal;   // ���� ���� ����
                fallCycle = 0.08f * (10 - levelVal); // �ӵ� ����
            }
            level.text = "" + levelVal;
            line.text = "" + lineVal;
        }

        // ��� �ִ� ���� �����ϸ� �Ʒ��� ������
        if (isCleared)
        {
            //���� �ٴ� ���� ���� �ʿ䰡 �����Ƿ� index 1 ���� for�� ����
            for (int i = 1; i < boardNode.childCount; ++i)
            {
                var column = boardNode.Find(i.ToString());

                // �̹� ��� �ִ� ���� ����
                if (column.childCount == 0)
                    continue;

                // ���� �� �Ʒ��ʿ� �� ���� �����ϴ��� Ȯ��, �� �ุŭ emptyCol ����
                int emptyCol = 0;
                int j = i - 1;
                while (j >= 0)
                {
                    if (boardNode.Find(j.ToString()).childCount == 0)
                    {
                        emptyCol++;
                    }
                    j--;
                }

                // ���� �� �Ʒ��ʿ� �� �� ����� �Ʒ��� ����
                if (emptyCol > 0)
                {
                    var targetColumn = boardNode.Find((i - emptyCol).ToString());

                    while (column.childCount > 0)
                    {
                        Transform tile = column.GetChild(0);
                        tile.parent = targetColumn;
                        tile.transform.position += new Vector3(0, -emptyCol, 0);
                    }
                    column.DetachChildren();
                }
            }
        }
    }

    // �̵� �������� üũ �� True or False ��ȯ�ϴ� �޼���
    bool CanMoveTo(Transform root)  // tetrominoNode�� �Ű����� root�� ��������
    {
        // tetrominoNode�� �ڽ� Ÿ���� ��� �˻�
        for (int i = 0; i < root.childCount; ++i)
        {
            var node = root.GetChild(i);
            // ����Ƽ ��ǥ�迡�� ��Ʈ���� ��ǥ��� ��ȯ
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);
            // �̵� ������ ��ǥ���� Ȯ�� �� ��ȯ
            if (x < 1 || x > boardWidth)
                return false;
            if (y < 0)
                return false;

            // �̹� �ٸ� Ÿ���� �ִ��� Ȯ��
            var column = boardNode.Find(y.ToString());

            if (column != null && column.Find(x.ToString()) != null)
                return false;
        }
        return true;
    }

    // Ÿ�� ����
    Tile CreateTile(Transform parent, Vector2 position, Color color, int order = 1)
    {
        var go = Instantiate(tilePrefab); // tilePrefab�� ������ ������Ʈ ����
        go.transform.parent = parent; // �θ� ����
        go.transform.localPosition = position; // ��ġ ����

        var tile = go.GetComponent<Tile>(); // tilePrefab�� Tile��ũ��Ʈ �ҷ�����
        tile.color = color; // ���� ����
        tile.sortingOrder = order; // ���� ����

        return tile;
    }
    // ��� Ÿ���� ����
    void CreateBackground()
    {
        Color color = Color.gray;   // ��� �� ����(���ϴ� ������ ���� ����)

        // Ÿ�� ����
        color.a = 0.6f; // �׵θ��� ���� ���� ������ �ٲٱ�
        for (int x = -halfWidth; x < halfWidth + 1; ++x)
        {
            for (int y = halfHeight; y > -halfHeight; --y)
            {
                CreateTile(backgroundNode, new Vector2(x, y), color, 0);
            }
        }

        // �¿� �׵θ�
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            CreateTile(backgroundNode, new Vector2(-halfWidth, y), color, 0);
            CreateTile(backgroundNode, new Vector2(halfWidth + 1, y), color, 0);
        }

        // �Ʒ� �׵θ�
        for (int x = -halfWidth; x <= halfWidth + 1; ++x)
        {
            CreateTile(backgroundNode, new Vector2(x, -halfHeight), color, 0);
        }
    }

    void CreatePreview()
    {
        // �̹� �ִ� �̸����� �����ϱ�
        foreach (Transform tile in previewNode)
        {
            Destroy(tile.gameObject);
        }
        previewNode.DetachChildren();

        indexVal = Random.Range(0, 7);

        Color32 color = Color.white;

        // �̸����� ��Ʈ�ι̳� ���� ��ġ
        previewNode.position = new Vector2(-halfWidth - 3, halfHeight - 4);

        switch (indexVal)
        {
            case 0: // I
                color = new Color32(115, 251, 253, 255);    // �ϴû�
                CreateTile(previewNode, new Vector2(0f, 2.0f), color);
                CreateTile(previewNode, new Vector2(0f, 1.0f), color);
                CreateTile(previewNode, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode, new Vector2(0f, -1.0f), color);
                break;

            case 1: // J
                color = new Color32(0, 33, 245, 255);    // �Ķ���
                CreateTile(previewNode, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode, new Vector2(1f, 0.0f), color);
                CreateTile(previewNode, new Vector2(-1f, 1.0f), color);
                break;

            case 2: // L
                color = new Color32(243, 168, 59, 255);    // ��Ȳ��
                CreateTile(previewNode, new Vector2(-1f, 0.0f), color);
                CreateTile(previewNode, new Vector2(0f, 0.0f), color);
                CreateTile(previewNode, new Vector2(1f, 0.0f), color);
                CreateTile(previewNode, new Vector2(1f, 1.0f), color);
                break;

            case 3: // O 
                color = new Color32(255, 253, 84, 255);    // �����
                CreateTile(previewNode, new Vector2(0f, 0f), color);
                CreateTile(previewNode, new Vector2(1f, 0f), color);
                CreateTile(previewNode, new Vector2(0f, 1f), color);
                CreateTile(previewNode, new Vector2(1f, 1f), color);
                break;

            case 4: //  S
                color = new Color32(117, 250, 76, 255);    // ���
                CreateTile(previewNode, new Vector2(-1f, -1f), color);
                CreateTile(previewNode, new Vector2(0f, -1f), color);
                CreateTile(previewNode, new Vector2(0f, 0f), color);
                CreateTile(previewNode, new Vector2(1f, 0f), color);
                break;

            case 5: //  T
                color = new Color32(155, 47, 246, 255);    // ���ֻ�
                CreateTile(previewNode, new Vector2(-1f, 0f), color);
                CreateTile(previewNode, new Vector2(0f, 0f), color);
                CreateTile(previewNode, new Vector2(1f, 0f), color);
                CreateTile(previewNode, new Vector2(0f, 1f), color);
                break;

            case 6: // Z
                color = new Color32(235, 51, 35, 255);    // ������
                CreateTile(previewNode, new Vector2(-1f, 1f), color);
                CreateTile(previewNode, new Vector2(0f, 1f), color);
                CreateTile(previewNode, new Vector2(0f, 0f), color);
                CreateTile(previewNode, new Vector2(1f, 0f), color);
                break;
        }
    }

    // ��Ʈ�ι̳� ����
    void CreateTetromino()
    {
        //���� ó���� ������ ��Ʈ�ι̳��ΰ��
        int index;
        if (indexVal == -1)
        {
            index = Random.Range(0, 7); // �������� 0~6 ������ �� ����
        }
        else index = indexVal;  // Preview�� �� ��������     

        Color32 color = Color.white;

        // ȸ�� ��꿡 ����ϱ� ���� ���ʹϾ� Ŭ����
        tetrominoNode.rotation = Quaternion.identity;
        // ��Ʈ�ι̳� ���� ��ġ (�߾� ���), �� ���� ����
        tetrominoNode.position = new Vector2(offset_x, halfHeight + offset_y);

        switch (index)
        {
            // ������ ���� ��Ʈ���� ��翡 ����ϰ� ����� ǥ�� (I, J, L ,O, S, T, Z)

            case 0: // I
                color = new Color32(112, 248, 224, 255);    // �ϴû�
                CreateTile(tetrominoNode, new Vector2(-2f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(-1f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0.0f), color);
                break;

            case 1: // J
                color = new Color32(0, 73, 245, 255);    // �Ķ���
                CreateTile(tetrominoNode, new Vector2(-1f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(-1f, 1.0f), color);
                break;

            case 2: // L
                color = new Color32(243, 168, 59, 255);    // ��Ȳ��
                CreateTile(tetrominoNode, new Vector2(-1f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0.0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 1.0f), color);
                break;

            case 3: // O 
                color = new Color32(251, 230, 0, 255);    // �����
                CreateTile(tetrominoNode, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 1f), color);
                break;

            case 4: //  S
                color = new Color32(117, 250, 76, 255);    // ���
                CreateTile(tetrominoNode, new Vector2(-1f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, -1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), color);
                break;

            case 5: //  T
                color = new Color32(155, 47, 246, 255);    // ���ֻ�
                CreateTile(tetrominoNode, new Vector2(-1f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), color);
                break;

            case 6: // Z
                color = new Color32(217, 49, 32, 255);    // ������
                CreateTile(tetrominoNode, new Vector2(-1f, 1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), color);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), color);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), color);
                break;
        }
        CreatePreview();
    }
}