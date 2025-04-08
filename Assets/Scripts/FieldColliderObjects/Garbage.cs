using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Garbage : PlatformColliderObject
{
    [SerializeField] private Transform rootGarbage;
    [SerializeField] private string garbageName;
    [SerializeField] private GameObject garbageVisual;
    [SerializeField] private GameObject bubbleVisual;
    [SerializeField] private ParticleSystem foamEffect;

    [SerializeField] private WorldType worldType;
    private Sequence sequenceVisual;

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
                        ((FMMainCharacter)character).OnHitGarbage(this);
                    }
                    break;
                case WorldType.Surf:
                    character = other.gameObject.GetComponentInParent<FMSurfCharacter>();
                    if (character != null)
                    {
                        ((FMSurfCharacter)character).OnHitGarbage(this);
                    }
                    break;
            }

            Show(false);
        }
    }

    public void SetGarbage(GameObject inGarbageVisual)
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        worldType = mainScene.GetCurrentWorldType();

        garbageVisual = inGarbageVisual;
        garbageName = garbageVisual.name;
        garbageVisual.transform.SetParent(rootGarbage);
        garbageVisual.transform.localPosition = Vector3.zero; 
        garbageVisual.transform.localRotation = Quaternion.identity; 

        switch (worldType)
        {
            case WorldType.Main:
                garbageVisual.transform.localPosition = Vector3.up * 1.25f;
                garbageVisual.gameObject.SetActive(true);
                bubbleVisual.SetActive(true);

                Vector3 targetRotationMain = new Vector3(0, 360f, 0);
                float targetPosMain = 1.5f;

                if (sequenceVisual == null)
                {
                    sequenceVisual = DOTween.Sequence();
                }

                sequenceVisual.Append(rootGarbage.DOLocalRotate(targetRotationMain, 2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear));
                sequenceVisual.Join(rootGarbage.DOLocalMoveY(targetPosMain, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad));
                sequenceVisual.Play();
                break;
            case WorldType.Surf:
                garbageVisual.transform.localPosition = Vector3.up * 2f;
                garbageVisual.transform.localRotation = Quaternion.identity;
                
                /*Transform garbageVisualChild = garbageVisual.transform.GetChild(0);
                Transform garbageVisualChildChild = garbageVisualChild.transform.GetChild(0);
                Renderer garbageRender = garbageVisualChildChild.GetComponent<Renderer>();

                Vector3 rightEdgePosition = garbageRender.localBounds.min;
                foamEffect.transform.localPosition += rightEdgePosition;

                foamEffect.gameObject.SetActive(true);*/
                bubbleVisual.SetActive(false);

                float targetPosSurf = 1.25f;
                rootGarbage.DOLocalMoveY(targetPosSurf, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);

                break;
        }
    }
        

    public string GetGarbageName()
    {
        string result = garbageName;
        return result;
    }

    public GameObject GetGarbageVisual()
    {
        GameObject result = garbageVisual;
        return result;
    }

    public void ClearGarbage()
    {
        garbageName = string.Empty;
        garbageVisual = null;
        
        if (sequenceVisual != null)
        {
            sequenceVisual.Kill();
            sequenceVisual = null;
        }
    }
}
