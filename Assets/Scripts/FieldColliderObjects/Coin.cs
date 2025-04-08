using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : PlatformColliderObject
{
    [SerializeField] Transform transformCoinModel;
    private GameObject currentCoinModel;
    private string coinName;
    private Vector3 initialPosition;
    private Tweener tweenRotate;

    public string CoinName
    {
        get
        {
            return coinName;
        }
        set
        {
            coinName = value;
        }
    }

    private const float ROTATE_SPEED = 2.275f;

    public override void OnHitCollision(Collision collision)
    {

    }

    public override void OnHitTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            VDCharacter character = null;

            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            WorldType worldType = mainScene.GetCurrentWorldType();

            switch (worldType)
            {
                case WorldType.Main:
                    character = other.gameObject.GetComponentInParent<FMMainCharacter>();
                    if (character != null)
                    {
                        ((FMMainCharacter)character).OnHitCoin(this);
                    }
                    break;
                case WorldType.Surf:
                    character = other.gameObject.GetComponentInParent<FMSurfCharacter>();
                    if (character != null)
                    {
                        ((FMSurfCharacter)character).OnHitCoin(this);
                    }
                    break;
            }

            Show(false);    
        }
        if (other.gameObject.CompareTag("Magnet"))
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            MoveToPlayer(playerTransform);
        }
    }

    public void ClearCoinModel()
    {
        if (transformCoinModel.childCount > 0)
        {
            FMPlatformController.Get().StoreStashCoinVisual(currentCoinModel);
        }
    }

    public void SetCoinModel(GameObject coinModel, ViewMode viewMode)
    {
        float randomValue = Random.Range(-0.3f, 0.3f);
        Vector3 randomPos = viewMode == ViewMode.SideView ? new Vector3(0, randomValue, 0) : new Vector3(0, 0, randomValue);
        transformCoinModel.transform.localPosition = randomPos;
        coinModel.transform.SetParent(transformCoinModel);
        coinModel.transform.localPosition = Vector3.zero;
        coinModel.gameObject.SetActive(true);
        currentCoinModel = coinModel;
    }

    public void SetCoinName(string name)
    {
        coinName = name;
    }

    public IEnumerator SetCoinRotation()
    {
        float yRotation = Random.Range(0, 360);
        Vector3 initRotation = new Vector3(0, yRotation, 0);
        Vector3 targetRotation = new Vector3(0, yRotation + 360, 0);
        transformCoinModel.transform.localEulerAngles = initRotation;

        if (tweenRotate != null)
        {
            tweenRotate.Restart();
            tweenRotate = null;
        }

        tweenRotate = transformCoinModel.DOLocalRotate(targetRotation, ROTATE_SPEED, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

        yield return null;
    }
    private void MoveToPlayer(Transform player)
    {
        if (player == null) return;

        float duration = 0.3f;
        transform.DOMove(player.position, duration).SetEase(Ease.OutQuint);
         

    }

    public void SetCoinOriginalPosition()
    {
        initialPosition = transform.position;
    }

    public void ResetCoinPosition()
    {
        transform.position = initialPosition;
    }
}
