﻿using System;
using System.Linq;
using DUCK.DebugMenu.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace DUCK.DebugMenu
{
	/// <summary>
	/// A script that provides access and a simple API to control the debug menu.
	///
	/// DUCK ships with a ready-made prefab with this script already configured, just drop it into your startup scene,
	/// then access it via the singleton.
	///
	/// To bring up the DebugMenu use an IDebugMenuSummoner and register it with the DebugMenu via AddSummoner,
	/// or add it to the same object as the debug menu, and it will find it on Start(). The ready-ade prefab that ships
	/// with DUCK, has a DefaultDebugMenuSummoner. If you want to use your own logic to summon the debug menu then
	/// implement your own IDebugMenuSummoner and register it.
	/// </summary>
	public class DebugMenu : MonoBehaviour
	{
		[SerializeField]
		private GameObject rootDebugContent;

		[SerializeField]
		private GameObject rootDebugButton;

		[SerializeField]
		private GameObject rootObject;

		[Header("Buttons")]
		[SerializeField]
		private Button closeButton;

		[SerializeField]
		private Button tabPageButtonTemplate;

		[Header("Pages")]
		[SerializeField]
		private DebugMenuActionsPage actionsPage;

		[SerializeField]
		private AbstractDebugMenuTabPage[] pages;

		private bool debugPressed;
		public event Action OnShow;
		public event Action OnHide;

		public bool DebugPressed
        {
            get
            {
				return debugPressed;
            }
            set
            {
				debugPressed = value;
            }
        }

		public void DoAwake()
		{
			rootDebugContent.gameObject.SetActive(false);
			rootDebugButton.gameObject.SetActive(true);

			actionsPage.Init();

			// When running tests you cannot use DontDestroyOnLoad in editor mode
			if (Application.isPlaying)
			{
				DontDestroyOnLoad(this);
			}
		}

		public void Init()
		{
			// Find any summoners added to the same object
			var summoners = GetComponents<IDebugMenuSummoner>();

			foreach (var summoner in summoners)
			{
				AddSummoner(summoner);
			}

			foreach (var page in pages)
			{
				if (!page.HasButton) continue;

				var tabButton = Instantiate(tabPageButtonTemplate, tabPageButtonTemplate.transform.parent);
				tabButton.transform.SetSiblingIndex(0);
				page.TabButton = tabButton;
				var buttonText = tabButton.GetComponentInChildren<Text>();
				buttonText.text = page.ButtonText;
				var thisPage = page;
				page.BackButton.onClick.AddListener(() => HandleTabClosed(thisPage));
				tabButton.onClick.AddListener(() => HandleTabButtonClicked(thisPage));
			}

			tabPageButtonTemplate.gameObject.SetActive(false);

			EnableAllTabs();
		}

		/// <summary>
		/// Shows the debug menu
		/// </summary>
		public void Show()
		{
			rootDebugContent.gameObject.SetActive(true);
			rootDebugButton.gameObject.SetActive(false);

			if (OnShow != null)
			{
				OnShow.Invoke();
			}
		}

		/// <summary>
		/// Hides the debug menu
		/// </summary>
		public void Hide()
		{
			debugPressed = false;
			rootDebugContent.gameObject.SetActive(false);
			rootDebugButton.gameObject.SetActive(true);

			if (OnHide != null)
			{
				OnHide.Invoke();
			}
		}

		/// <summary>
		/// Adds a new debug menu summoner
		/// </summary>
		public void AddSummoner(IDebugMenuSummoner summoner)
		{
			if (summoner == null) throw new ArgumentNullException("summoner");
			summoner.OnSummonRequested += Show;
		}

		/// <summary>
		/// Removes a debug menu summoner
		/// </summary>
		public void RemoveSummoner(IDebugMenuSummoner summoner)
		{
			if (summoner == null) throw new ArgumentNullException("summoner");
			summoner.OnSummonRequested -= Show;
		}

		/// <summary>
		/// Adds a new button to the debug menu that will invoke the specified action when clicked
		/// </summary>
		/// <param name="path">The path for the button, must be unique</param>
		/// <param name="action">The action to invoke when clicked</param>
		/// <param name="hideDebugMenuOnClick">A boolean used to control if the debug menu should hide when the button is clicked (defaults to true)</param>
		public void AddButton(string path, Action action, bool hideDebugMenuOnClick = false)
		{
			actionsPage.AddButton(path, action, hideDebugMenuOnClick);
		}

		/// <summary>
		/// Removes a button from the debug menu.
		/// </summary>
		/// <param name="path">The path of of the button to remove</param>
		public void RemoveButton(string path)
		{
			actionsPage.RemoveButton(path);
		}

		/// <summary>
		/// Gets a specific page from the debug menu by type
		/// </summary>
		/// <typeparam name="TPage">The type of page to return</typeparam>
		/// <returns>The page if it exists or null if it does not</returns>
		public TPage GetPage<TPage>() where TPage : AbstractDebugMenuTabPage
		{
			return pages.FirstOrDefault(p => p is TPage) as TPage;
		}

		private void HandleTabButtonClicked(AbstractDebugMenuTabPage page)
		{
			foreach (var otherPage in pages)
			{
				otherPage.gameObject.SetActive(otherPage == page);
				if (otherPage.TabButton != null)
				{
					otherPage.TabButton.interactable = otherPage != page;
				}
			}
		}

		private void HandleTabClosed(AbstractDebugMenuTabPage page)
		{
			page.gameObject.SetActive(false);
			EnableAllTabs();
		}

		private void EnableAllTabs()
		{
			foreach (var otherPage in pages)
			{
				if (otherPage.TabButton != null)
				{
					otherPage.TabButton.interactable = true;
				}
			}
		}
	}
}

