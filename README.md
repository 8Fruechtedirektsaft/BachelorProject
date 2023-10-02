# BachelorProject
This project implements the different Cellular Automata explained in the thesis below. 
The different parts of the map are generated and visualized.
"Untersuchung der Nutzung von Cellular Automata zur Generierung von Terrain-Karten für Videospiele" 
URL: https://nbn-resolving.org/urn:nbn:de:bsz:l189-qucosa2-872097

I recommend to test the whole project because the application does not allow any changes to the parameters.

controls:
Return - generate the whole map (takes a moment due to visualisation)

(press once in order)
1 - generate climate zones
2 - generate a height map
3 - generate rivers
4 - generate forests
5 - generate ressources
6 - generate settlements

(press multiple times and then in order)
Q - do one step in generating climate zones
W - do one step in generating a height map
E - do some steps in generating rivers
R - do some steps in generating forests
T - do some steps in generating ressources
Z - do some steps in generating settlements

C - clear the maps generated after pressing 1, 2, Q and W
Y - remove some ressource generated after pressing 5, T

code summary:
Assets/Scripts/MyGameManager
-takes user input and generates the according parts of the map

Assets/Scripts/HexGridHelper
-methods to navigate a pointy top hexagonal grid
-calculates grid to world space

Assets/Scripts/Data/BaseMapGenerator
-methods to fill a grid with values

Assets/Scripts/Data/HexTerrain
-contains the different cellular automata that represent parts of the map
-controls the updating of the map parts

Assets/Scripts/Data/HexMapData
-initializes the different cellular automata based on the chosen parameters
-links MyGameManager to the update methods of HexTerrain
-calls the DataViewAdapter to update the visualisation and gives it the necessary data

Assets/Scripts/Data/CA/...
-contains the different cellular automata described in the thesis as individual classes
-contains the data types needed for some of the cellular automata

Assets/Scripts/View/DataViewAdapter
-takes in the information of the map state from HexMapData
-generates the cells of the map by instantiating the according prefabs 

Assets/Scripts/View/HexMapView
-contains the objects instantiated by DataViewAdapter