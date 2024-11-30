using System.Collections;
using UnityEngine;

public class SpriteController : MonoBehaviour {
    private SpriteSwitcher switcher;
    private RectTransform rect;
    private Animator animator;

    public void Awake(){
        switcher = GetComponent<SpriteSwitcher>();
        animator = GetComponent<Animator>();
        rect = GetComponent<RectTransform>();
    }

    public void Setup(Sprite sprite){
        switcher.SetImage(sprite);
    }

    public void Show(Vector2 coords){
        animator.SetTrigger("ShowCharacter");
        rect.localPosition = coords;
    }

    public void Hide(){
        animator.SetTrigger("HideCharacter");
    }

    public void Move(Vector2 coords, float speed){
        StartCoroutine(MoveCoroutine(coords, speed));
    }

    private IEnumerator MoveCoroutine(Vector2 coords, float speed){
        while (rect.localPosition.x != coords.x || rect.localPosition.y != coords.y){
            rect.localPosition = Vector2.MoveTowards(rect.localPosition, coords, Time.deltaTime * 1000f * speed);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void SwitchSprite(Sprite sprite){
        if (switcher.GetImage() != sprite){
            switcher.SwitchImage(sprite);
        }
    }
}