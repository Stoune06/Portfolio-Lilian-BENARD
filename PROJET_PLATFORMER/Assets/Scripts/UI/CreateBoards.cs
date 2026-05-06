using UnityEngine;

public class CreateBoards : MonoBehaviour
{
    [SerializeField] private GameObject _Board;
    [SerializeField] private Transform _BoardContainer;
    [SerializeField] private GameObject _ErrorBoard;

    private void Start()
    {
        Leaderboard();
    }

    private async void Leaderboard()
    {
        SaveData[] lDatas = await DataManager.Instance.GetLeaderboard();

        if (lDatas != null)
        {
            int lNum = lDatas.Length;
            Debug.Log(lNum);
            for (int i = 0; i < lNum; i++)
            {
                GameObject lBoard = Instantiate(_Board, _BoardContainer);
                LoadBoard lLoader = lBoard.GetComponent<LoadBoard>();

                lLoader.Load(lDatas[i].username, lDatas[i].collectibles.ToString(), lDatas[i].currentLevel.ToString(), lDatas[i].lastCheckpoint.ToString());
                Debug.Log(lDatas[i].username);
                //lBoard.transform.position += Vector3.up * (-i * 160);

            }
            _BoardContainer.position += Vector3.down * 2000;
        }
        else _ErrorBoard.SetActive(true);
    }

}
