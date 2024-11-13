using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDirector : MonoBehaviour
{
    public StoryScene currentScene;
    public DialogueController DC;
    public BackgroundController BC;
    private void Start()
    {
        DC.PlayScene(currentScene);
        BC.SetImage(currentScene.background);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            if (DC.IsCompleted())
            {
                if (DC.IsLastSentence())
                {
                    currentScene = currentScene.nextScene;
                    DC.PlayScene(currentScene);
                    BC.SwitchImage(currentScene.background);

                }
                else
                    DC.PlayNextSentence();
            }


    }

}
