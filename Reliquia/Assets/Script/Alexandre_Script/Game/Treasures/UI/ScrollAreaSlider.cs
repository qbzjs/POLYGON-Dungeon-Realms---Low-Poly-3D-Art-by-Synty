using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace AlexandreTreasures
{
	public class ScrollAreaSlider : MonoBehaviour
	{
		#region Fields

		[SerializeField] private RectTransform _viewport = default;
		[SerializeField] private RectTransform _content = default;
		[SerializeField] private GameObject _slider = default;
		[SerializeField] private Slider _sliderUI = default;

		[SerializeField] private VerticalLayoutGroup _verticalLayoutGroup = default;

		[Tooltip("La hauteur d'un bouton calculée au Runtime selon son RectTransform et le Spacing du VerticalLayoutGroup.")]
		[SerializeField] private float _btnHeight; // 57.2 actuellement

		[SerializeField] private RectTransform _btnReference = default;

		private bool _hasCheckedValue;

		private float _btnNumbers;

		#endregion



		#region Frame cycle

		private void Awake()
		{
			_btnHeight = _verticalLayoutGroup.spacing + _btnReference.rect.height;
		}

		private void OnEnable()
		{
			// Toujours commencer à 0 lorsque l'objet est activé
			_sliderUI.value = 0.0f;
		}

		private void OnDisable()
		{
			// Lors de la désactivation, réinitialiser l'interrupteur et le slider
			_hasCheckedValue = false;
			_slider.SetActive(true);
		}

		private void Update()
		{
			if (!_hasCheckedValue)
			{
				// sizeDelta.y n'est disponible qu'à la seconde frame et non la première
				// Donc, on ne peut faire de calculs que si sa valeur est différente de 0
				if (_content.sizeDelta.y != 0)
				{
					_btnNumbers =  GetBtnNumbers();

					if (_btnNumbers < 1)
					{
						_slider.SetActive(false);
					}

					_hasCheckedValue = true;
				}
			}
		}

		#endregion



		#region Public methods

		public void MovingContent(float value)
		{
			_btnNumbers = GetBtnNumbers();

			int currentStep = Mathf.RoundToInt(value / (1f / _btnNumbers));

			float currentStepHeight = currentStep * _btnHeight;

			_content.anchoredPosition = new Vector2(0, currentStepHeight);
		}

		#endregion



		#region Private methods

		private float GetBtnNumbers()
		{
			return (_content.sizeDelta.y - _viewport.sizeDelta.y) / _btnHeight;
		}

		#endregion
	}
}