using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwitch : MonoBehaviour
{
    AudioSource chessSoundSource;
    [SerializeField] internal AudioClip chessClick;
    [SerializeField] internal AudioClip chessMove;

    // Start is called before the first frame update
    void Start()
    {
        chessSoundSource = gameObject.GetComponent<AudioSource>();
        chessSoundSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
        Chess.moveSounds = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayChessmanSound(bool isMove)
    {
        if(isMove)
        {
            chessSoundSource.volume = 0.8f;
            chessSoundSource.clip = chessMove;

        } else
        {
            chessSoundSource.volume = 0.4f;
            chessSoundSource.clip = chessClick;

        }
        chessSoundSource.Play();
    }
}
