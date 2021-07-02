using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace AlexandreTreasures
{
	public class TreasuresPanelClick : MonoBehaviour, IPointerClickHandler // Event propagation
	{
		#region Fields

		[SerializeField] private GameObject _thisGameObject = default;

		[SerializeField] private Image _image = default;
		[SerializeField] private Sprite _imagePlaceholderSprite = default;
		private Color _imagePlaceholderColor;

		[SerializeField] private RectTransform _content = default;

		[SerializeField] private TreasuresData _treasuresData = default;

		[SerializeField] private RectTransform _btnReference = default;
		//private TextMeshProUGUI _btnReferenceText;
		private Color _defaultBtnReferenceTextColor = default;

		private List<TreasureBtn> m_treasureBtnList = new List<TreasureBtn>(); // liste des composants de bouton

		private int _currentTreasureBtn = -1; // valeur ne correspondant à aucun bouton

		#endregion



		#region Frame cycle

		private void Awake()
		{
			// Conserver des éléments en référence
			_defaultBtnReferenceTextColor = _btnReference.GetComponentInChildren<TextMeshProUGUI>().color;
			_imagePlaceholderColor = _image.color;

			// Créer la liste des boutons de trésor dans le ScrollArea d'UI et conserver la liste de ces boutons
			for (int i = 0; i < _treasuresData.m_treasuresList.Count; i++)
			{
				RectTransform rectButton = Instantiate(_btnReference, _content);

				rectButton.gameObject.name = _btnReference.name;
				rectButton.gameObject.SetActive(true);

				TreasureBtn tb = rectButton.GetComponent<TreasureBtn>();
				tb.m_treasureInfo = _treasuresData.m_treasuresList[i];

				m_treasureBtnList.Add(tb);
			}
		}

		private void OnEnable()
		{
			// Par défaut, le texte des boutons est "???" ou autre du même genre.
			// On veut changer le texte si le trésor a été découvert.
			// Et si c'est fait, ne pas réeffectuer le test.
			foreach (TreasureBtn item in m_treasureBtnList)
			{
				if (!item.m_isDone && item.m_treasureInfo.m_isDiscovered)
				{
					item.m_text.text = item.m_treasureInfo.m_name;
					item.m_isDone = true;
				}

				// Le texte a sa couleur par défaut
				item.m_text.color = _defaultBtnReferenceTextColor;
			}

			// L'image a par défaut le sprite de placeholoder et sa couleur 
			_image.sprite = _imagePlaceholderSprite;
			_image.color = _imagePlaceholderColor;
		}

		#endregion



		#region Public methods

		// Méthode de IPointerClickHandler
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.pointerCurrentRaycast.gameObject.name == _btnReference.name)
			{
				TreasureBtn treasureBtn = eventData.pointerCurrentRaycast.gameObject.GetComponent<TreasureBtn>();

				if (treasureBtn.m_treasureInfo.m_isDiscovered)
				{
					_image.sprite = treasureBtn.m_treasureInfo.m_sprite;

					_image.color = Color.white; // pleine couleur

					if (_currentTreasureBtn != -1)
					{
						m_treasureBtnList[_currentTreasureBtn].m_text.color = _defaultBtnReferenceTextColor;
					}

					treasureBtn.m_text.color = Color.white;

					_currentTreasureBtn = m_treasureBtnList.IndexOf(treasureBtn);
				}
			}
		}

		public void ToggleTreasuresPanel()
		{
			_thisGameObject.SetActive(!_thisGameObject.activeInHierarchy);
		}

		#endregion
	}
}