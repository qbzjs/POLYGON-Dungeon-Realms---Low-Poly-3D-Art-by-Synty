using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ES3Internal;


/*
 * ---- How Postprocessing works for the reference manager ----
 * - When the manager is first added to the scene, all top-level dependencies are added to the manager (AddManagerToScene).
 * - When the manager is first added to the scene, all prefabs with ES3Prefab components are added to the manager (AddManagerToScene).
 * - All GameObjects and Components in the scene are added to the reference manager when we enter Playmode or the scene is saved (PlayModeStateChanged, OnWillSaveAssets -> AddGameObjectsAndComponentstoManager).
 * - When a UnityEngine.Object field of a Component is modified, the new UnityEngine.Object reference is added to the reference manager (PostProcessModifications)
 * - All prefabs with ES3Prefab Components are added to the reference manager when we enter Playmode or the scene is saved (PlayModeStateChanged, OnWillSaveAssets -> AddGameObjectsAndComponentstoManager).
 * - Local references for prefabs are processed whenever a prefab with an ES3Prefab Component is deselected (SelectionChanged -> ProcessGameObject)
 */
[InitializeOnLoad]
public class ES3Postprocessor : UnityEditor.AssetModificationProcessor
{
	public static ES3ReferenceMgr refMgr
	{
		get{ return (ES3ReferenceMgr)ES3ReferenceMgr.Current; }
	}
	
	public static ES3AutoSaveMgr autoSaveMgr
	{
		get{ return ES3AutoSaveMgr.Instance; }
	}

	public static GameObject lastSelected = null;


	// This constructor is also called once when playmode is activated and whenever recompilation happens
    // because we have the [InitializeOnLoad] attribute assigned to the class.
	static ES3Postprocessor()
	{
		// Open the Easy Save 3 window the first time ES3 is installed.
		ES3Editor.ES3Window.OpenEditorWindowOnStart();

		//Selection.selectionChanged += SelectionChanged;
        //Undo.postprocessModifications += PostProcessModifications;

#if UNITY_2017_2_OR_NEWER
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
#else
        EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
#endif
    }

#region Reference Updating

    private static void RefreshReferences(bool isEnteringPlayMode = false)
    {
        if (refMgr != null && ES3Settings.defaultSettingsScriptableObject.autoUpdateReferences)
            refMgr.RefreshDependencies(isEnteringPlayMode);
        UpdateAssembliesContainingES3Types();
    }

#if UNITY_2017_2_OR_NEWER
    public static void PlayModeStateChanged(PlayModeStateChange state)
    {
        // Add all GameObjects and Components to the reference manager before we enter play mode.
        if (state == PlayModeStateChange.ExitingEditMode && ES3Settings.defaultSettingsScriptableObject.autoUpdateReferences)
            RefreshReferences(true);
    }
#else
    public static void PlaymodeStateChanged()
    {
        // Add all GameObjects and Components to the reference manager before we enter play mode.
        if (!EditorApplication.isPlaying && ES3Settings.defaultSettingsScriptableObject.autoUpdateReferences)
            RefreshReferences(true);
    }
#endif


    public static string[] OnWillSaveAssets(string[] paths)
    {
        RefreshReferences();
        return paths;
    }

    /*static UndoPropertyModification[] PostProcessModifications(UndoPropertyModification[] modifications)
    {
        if (refMgr != null && ES3Settings.defaultSettingsScriptableObject.autoUpdateReferences)
            // For each property which has had an Undo registered ...
            foreach (var mod in modifications)
                // If this property change is an object reference, and the Component this change has been made to is in the scene, not in Assets ...
                if (mod.currentValue != null && mod.currentValue.objectReference != null && mod.currentValue.target != null && !AssetDatabase.Contains(mod.currentValue.target))
                        // Add it to the reference manager
                        refMgr.Add(mod.currentValue.objectReference);
        return modifications;
    }*/

#endregion

    /*static void SelectionChanged()
	{
		if(EditorApplication.isPlaying || EditorApplication.isCompiling || EditorApplication.isPaused || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isUpdating)
			return;

        if (!ES3Settings.defaultSettingsScriptableObject.autoUpdateReferences)
            return;

        try
        {
            var selected = Selection.activeGameObject;

            // If we just deselected a prefab, process its references.
            if (ES3Settings.defaultSettingsScriptableObject.autoUpdateReferences && lastSelected != null && ES3EditorUtility.IsPrefabInAssets(selected))
                ProcessGameObject(lastSelected);

            lastSelected = selected;
        }
        catch{}
	}*/

    private static void UpdateAssembliesContainingES3Types()
    {
#if UNITY_2017_3_OR_NEWER
        var assemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
        var defaults = ES3Settings.defaultSettingsScriptableObject;
        var assemblyNames = new List<string>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var name = assembly.name;
                var substr = name.Length >= 5 ? name.Substring(0, 5) : "";

                if (substr != "Unity" && substr != "com.u" && !name.Contains("-Editor"))
                    assemblyNames.Add(name);
            }
            catch { }
        }

        defaults.settings.assemblyNames = assemblyNames.ToArray();
        EditorUtility.SetDirty(defaults);
#endif
    }

	private static void ProcessGameObject(GameObject go)
	{
		if(go == null) return;
		
		if(ES3EditorUtility.IsPrefabInAssets(go))
		{
			var es3Prefab = go.GetComponent<ES3Prefab>();
			if(es3Prefab != null)
				es3Prefab.GeneratePrefabReferences();
		}
		else if(refMgr != null)
			refMgr.AddDependencies(go);
	}

    public static GameObject AddManagerToScene()
    {
        var mgr = GameObject.Find("Easy Save 3 Manager");

        if (mgr == null)
        {
            mgr = new GameObject("Easy Save 3 Manager");
            var inspectorInfo = mgr.AddComponent<ES3InspectorInfo>();
            inspectorInfo.message = "The Easy Save 3 Manager is required in any scenes which use Easy Save, and is automatically added to your scene when you enter Play mode.\n\nTo stop this from automatically being added to your scene, go to 'Window > Easy Save 3 > Settings' and deselect the 'Auto Add Manager to Scene' checkbox.";
        }

        if (mgr.GetComponent<ES3ReferenceMgr>() == null)
        {
            mgr.AddComponent<ES3ReferenceMgr>().RefreshDependencies();
            refMgr.RefreshDependencies();
        }

        if (mgr.GetComponent<ES3AutoSaveMgr>() == null)
            mgr.AddComponent<ES3AutoSaveMgr>();

        Undo.RegisterCreatedObjectUndo(mgr, "Enabled Easy Save for Scene");
        return mgr;
    }
}