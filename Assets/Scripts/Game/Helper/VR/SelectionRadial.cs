using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace VRStandardAssets.Utils
{
    // This class is used to control a radial bar that fills
    // up as the user holds down the Fire1 button.  When it has
    // finished filling it triggers an event.  It also has a
    // coroutine which returns once the bar is filled.、
    // TODO,此类应该放在相应的按钮触发;
    public class SelectionRadial : MonoBehaviour
    {
        public event Action OnSelectionComplete;                        // This event is triggered when the bar has filled.

        [SerializeField] private float _SelectionDuration = 2f;         // How long it takes for the bar to fill.
        [SerializeField] private bool _HideOnStart = true;              // Whether or not the bar should be visible at the start.
        [SerializeField] private Image _Selection;                      // Reference to the image who's fill amount is adjusted to display the bar.
        [SerializeField] private VRInput _VRInput;                      // Reference to the VRInput so that input events can be subscribed to.
        
        private Coroutine _SelectionFillRoutine;                        // Used to start and stop the filling coroutine based on input.
        private bool _IsSelectionRadialActive;                          // Whether or not the bar is currently useable.
        private bool _RadialFilled;                                     // Used to allow the coroutine to wait for the bar to fill.

        public float SelectionDuration 
        { 
            get 
            {
                return _SelectionDuration; 
            } 
        }

        private void OnEnable()
        {
            _VRInput.OnDown += HandleDown;
            _VRInput.OnUp += HandleUp;
        }

        private void OnDisable()
        {
            _VRInput.OnDown -= HandleDown;
            _VRInput.OnUp -= HandleUp;
        }

        private void Start()
        {
            // Setup the radial to have no fill at the start and hide if necessary.
            _Selection.fillAmount = 0f;

            if(_HideOnStart)
                Hide();
        }

        public void Show()
        {
            _Selection.gameObject.SetActive(true);
            _IsSelectionRadialActive = true;
        }

        public void Hide()
        {
            _Selection.gameObject.SetActive(false);
            _IsSelectionRadialActive = false;

            // This effectively resets the radial for when it's shown again.
            _Selection.fillAmount = 0f;            
        }

        private IEnumerator FillSelectionRadial()
        {
            // At the start of the coroutine, the bar is not filled.
            _RadialFilled = false;

            // Create a timer and reset the fill amount.
            float timer = 0f;
            _Selection.fillAmount = 0f;
            
            // This loop is executed once per frame until the timer exceeds the duration.
            while (timer < _SelectionDuration)
            {
                // The image's fill amount requires a value from 0 to 1 so we normalise the time.
                _Selection.fillAmount = timer / _SelectionDuration;

                // Increase the timer by the time between frames and wait for the next frame.
                timer += Time.deltaTime;
                yield return null;
            }

            // When the loop is finished set the fill amount to be full.
            _Selection.fillAmount = 1f;

            // Turn off the radial so it can only be used once.
            _IsSelectionRadialActive = false;

            // The radial is now filled so the coroutine waiting for it can continue.
            _RadialFilled = true;

            // If there is anything subscribed to OnSelectionComplete call it.
            if (OnSelectionComplete != null)
                OnSelectionComplete();
        }

        public IEnumerator WaitForSelectionRadialToFill ()
        {
            // Set the radial to not filled in order to wait for it.
            _RadialFilled = false;

            // Make sure the radial is visible and usable.
            Show ();

            // Check every frame if the radial is filled.
            while (!_RadialFilled)
            {
                yield return null;
            }

            // Once it's been used make the radial invisible.
            Hide ();
        }
        
        private void HandleDown()
        {
            // If the radial is active start filling it.
            if (_IsSelectionRadialActive)
            {
                _SelectionFillRoutine = StartCoroutine(FillSelectionRadial());
            }
        }

        private void HandleUp()
        {
            // If the radial is active stop filling it and reset it's amount.
            if (_IsSelectionRadialActive)
            {
                if(_SelectionFillRoutine != null)
                    StopCoroutine(_SelectionFillRoutine);

                _Selection.fillAmount = 0f;
            }
        }
    }
}
