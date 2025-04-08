using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpinWheelGarbage : PlatformColliderObject
{
    [SerializeField] private Transform rootVisual;
    [SerializeField] private Transform rootHitbox;
    private Vector3 originalLocalPosition;
    private float zoomDuration = 0.3f;
    private bool isTouched;

    private FMWorld currentWorld;
    private FMMainCharacter mainCharacter;

    private void Start()
    {
        RandomFirst();
        GiveRotation();

        originalLocalPosition = rootVisual.localPosition;
    }

    void Update()
    {
        if (isTouched && zoomDuration > 0)
        {
            if (mainCharacter.CharacterViewMode == ViewMode.SideView)
            {
                zoomDuration -= Time.deltaTime;
            }
            else
            {
                zoomDuration -= Time.deltaTime;
            }
        }

        if (isTouched && zoomDuration < 0) //delete the object and spawn circle randomly
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                //spawn the wheel
                /*GameObject uiSpinWheel = GameObject.FindGameObjectWithTag("WheelSpin");
                int slotType = Random.Range(3, 7);
                uiSpinWheel.GetComponent<UIMiniGameSpinWheel>().SpawnWheel(gameObject.name, slotType);
                uiSpinWheel.GetComponent<UIMiniGameSpinWheel>().Show(true);*/

                isTouched = false;
                zoomDuration = 0.3f;
            }
        }
    }

    public override void OnHitCollision(Collision collision)
    {

    }

    public override void OnHitTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mainCharacter = other.gameObject.GetComponentInParent<FMMainCharacter>();
            if (mainCharacter != null)
            {
                isTouched = true;
                rootVisual.gameObject.SetActive(false);
                rootHitbox.gameObject.SetActive(false);
                mainCharacter.OnHitSpinGarbage(this, gameObject.name);
            }
            /*other.transform.GetComponentInParent<FMMainCharacter>().DisableOrEnableJump(true);
            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            currentWorld = mainScene.GetCurrentWorldObject();
            MainWorld mainWorld = currentWorld as MainWorld;
            mainWorld.PauseGame();
            MoveGarbageCenter();*/
        }
    }

    public void ResetModel()
    {
        rootVisual.localPosition = originalLocalPosition;
        gameObject.SetActive(true);
        rootVisual.gameObject.SetActive(true);
        rootHitbox.gameObject.SetActive(true);
    }

    private void GiveRotation()
    {
        float yRotation = Random.Range(0, 360);
        Vector3 initRotation = new Vector3(0, yRotation, 0);
        Vector3 targetRotation = new Vector3(0, yRotation + 360, 0);

        rootVisual.localEulerAngles = initRotation;
        rootVisual.DOLocalRotate(targetRotation, 2.275f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    public void RandomFirst()
    {
        int randomTrash = Random.Range(0, 7);
        GameObject garbage = null;

        switch (randomTrash)
        {
            case 0:
                garbage = FMAssetFactory.GetCoinVisual("Battery");
                gameObject.name = "Battery";
                break;
            case 1:
                garbage = FMAssetFactory.GetCoinVisual("GlassBottle");
                gameObject.name = "GlassBottle";
                break;
            case 2:
                garbage = FMAssetFactory.GetCoinVisual("PlasticBag");
                gameObject.name = "PlasticBag";
                break;
            case 3:
                garbage = FMAssetFactory.GetCoinVisual("PlasticBottle");
                gameObject.name = "PlasticBottle";
                break;
            case 4:
                garbage = FMAssetFactory.GetCoinVisual("PaperCup");
                gameObject.name = "PaperCup";
                break;
            case 5:
                garbage = FMAssetFactory.GetCoinVisual("SodaCan");
                gameObject.name = "SodaCan";
                break;
            case 6:
                garbage = FMAssetFactory.GetCoinVisual("MilkCarton");
                gameObject.name = "MilkCarton";
                break;
        }

        garbage.transform.SetParent(rootVisual);
        garbage.transform.localPosition = Vector3.zero;
        garbage.transform.localEulerAngles = Vector3.zero;
        garbage.transform.localScale = Vector3.one;
    }

    public void MoveGarbageCenter()
    {
        FMUnityMainCamera camera = FMSceneController.Get().Camera;
        MainWorld mainWorld = currentWorld as MainWorld;
        FMMainCharacter character = mainWorld.GetCharacter() as FMMainCharacter;
        Camera cam = camera.Camera;
        if (character.CharacterViewMode == ViewMode.SideView)
        {
            Vector3 middle = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            middle = new Vector3(middle.x + 1, middle.y - 2, middle.z + 13);
            rootVisual.DOMove(middle, 0.3f);
        }
        else
        {
            Vector3 middle = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            middle = new Vector3(middle.x, middle.y, middle.z + 5);
            rootVisual.DOMove(middle, 0.3f);
        }
    }
}
