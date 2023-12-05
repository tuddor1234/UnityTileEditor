using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Linq;
public class TileEditor : EditorWindow
{
    [MenuItem("Window/TileEditor/Editor")]
    public static void ShowExample()
    {
        TileEditor wnd = GetWindow<TileEditor>();
        wnd.titleContent = new GUIContent("TileEditor");
    }

    public IntegerField gridWidth;    
    public IntegerField gridHeight;

    public Button CreateLevelButton;
    public Button SaveLevelButton;
    public Button LoadLevelButton;

    public ListView list;
    List<Sprite> tiles = new List<Sprite>();
   // [SerializeField]
    private GameObject levelBuilderTemplate;
    private LevelBuilder builder = null;
    private LevelBuilder previousBuilder = null;

    private int padding = 10;
    public static int LevelNameGenerator = 0;

    public void CreateGUI()
    {
        LoadAssetsFromDisk();

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        root.style.alignSelf = Align.Stretch;
        root.style.alignItems = Align.Stretch;
        root.style.height = new Length(100, LengthUnit.Percent);
        root.style.alignContent = Align.Stretch;
        VisualElement layout = new VisualElement();
        layout.style.flexDirection = FlexDirection.Row;
        layout.style.marginBottom = padding;
        layout.style.paddingRight = padding;
        layout.style.width = new Length(100, LengthUnit.Percent);

        CreateLevelButton = new Button();
        CreateLevelButton.style.width = new Length(100, LengthUnit.Percent);
        CreateLevelButton.Add(new Label("Create Level"));
        CreateLevelButton.clicked += CreateNewLevel;
        layout.Add(CreateLevelButton);

        VisualElement LevelSeparator = new VisualElement();
        LevelSeparator.style.alignSelf = Align.Stretch;
        LevelSeparator.style.backgroundColor = Color.black;
        LevelSeparator.Add(new Label("Level Actions"));
        root.Add(LevelSeparator);
        root.Add(layout);

        // when the ListView needs more items to render
        Func<VisualElement> makeItem = () =>
        {
            VisualElement visualElement = new VisualElement();
            visualElement.style.flexDirection = FlexDirection.Row;
            visualElement.style.minHeight = new Length(40);
            visualElement.style.alignContent = Align.Stretch;
            visualElement.style.alignItems = Align.Stretch;
            visualElement.style.width = new Length(100, LengthUnit.Percent);

            var label = new Label();
            label.style.width = 35;
            label.style.marginRight = 100;
            label.style.marginLeft = 10;
            label.style.alignSelf = Align.Center;
            visualElement.Add(label);
            
            var image = new Image();
            image.style.width = 35;
            image.style.height = 35;
            image.style.alignSelf = Align.Center;
            visualElement.Add(image);
            return visualElement;
        };

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            List<VisualElement> children = (List<VisualElement>)e.Children();
          
            (children[0] as Label).text = tiles[i].name;
            (children[1] as Image).image = tiles[i].texture;
            e.name = tiles[i].name;
        };

        var listView = new ListView();
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemsSource = tiles;
        listView.style.alignContent = Align.Stretch;
        listView.style.alignItems = Align.Stretch;
        listView.style.minWidth = new Length(100);
        listView.selectionType = SelectionType.Single;
       // listView.style.maxWidth = new Length(500);
        listView.style.height = new Length(50, LengthUnit.Percent);
        // Callback invoked when the user double clicks an item
        listView.onSelectedIndicesChange += (objects) =>
        { 
            builder?.SelectTile(objects.First());          
        };
       

        VisualElement separator = new VisualElement();
        separator.style.alignSelf = Align.Stretch;
        separator.style.backgroundColor = Color.black;
        separator.Add(new Label("Tiles (Left Click to Add, Right Click to Remove)"));
        root.Add(separator);
        root.Add(listView);




        if(builder != null)
        {
            VisualElement builderSeparator = new VisualElement();
            builderSeparator.style.alignSelf = Align.Stretch;
            builderSeparator.style.backgroundColor = Color.black;
            builderSeparator.Add(new Label("Level Builder"));
            root.Add(builderSeparator);
      


            var scrollview = new ScrollView
            {
                viewDataKey = "WindowScrollView"
            };

            var inspector = new InspectorElement(builder);
            scrollview.Add(inspector);
            root.Add(scrollview);
        }


        
    }


    private void LoadAssetsFromDisk()
    {
        tiles.Clear();
        var sprites = Resources.LoadAll("Tiles", typeof(Sprite));
        tiles.AddRange(sprites);

        levelBuilderTemplate = (GameObject) Resources.Load("LevelBuilder", typeof(GameObject));

    }


    private void OnGUI()
    {
        if(Selection.activeGameObject != null)
        {
           var selectedBuilder = Selection.activeGameObject.GetComponent<LevelBuilder>();
            if (builder != selectedBuilder)
            {
                builder = selectedBuilder;
            }
        }

        if(builder != previousBuilder)
        {
            if (previousBuilder != null)
            {
                previousBuilder.bGridVisible = false;
            }
            if (builder != null)
            {
                builder.bGridVisible = true;
            }
            rootVisualElement.Clear();
            CreateGUI();
            previousBuilder = builder;
        }


    }

    private void CreateNewLevel()
    {
        GameObject go = Instantiate(levelBuilderTemplate);
        go.name = "Level" + LevelNameGenerator++;
        builder = go.GetComponent<LevelBuilder>();
        builder.AvaiableTiles = tiles;
        builder.bIsEditing = true;
    }

    private void OnDisable()
    {
        if (previousBuilder != null)
        {
            previousBuilder.bIsEditing = false;
        }

    }

}