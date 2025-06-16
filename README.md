# Material Property Drawers
A set of extra Material Property Drawers to use with Shaders and Shader Graphs.

## Common Attributes
Several Material Property Drawers share the same behaviors as they inherit from CustomMaterialPropertyDrawerBase.

`[VisibleIf(_Property)]`
Makes the property visible only if the referenced property is not null or non zero.
`[VisibleIf(_Property, true)]`
Hides the property if the referenced property is not null or non zero.

`[EnableIf(_Property)]`
Makes the property enabled only if the referenced property is not null or non zero.
`[EnableIf(_Property, true)]`
Disables the property if the referenced property is not null or non zero.

## Property Drawers
### Math Operators
Those work on Float and Range ShaderLab properties.
In ShaderGraph, that means a Float in Default or Slider mode.
Note if set to Slider, the limits are those expected on the slider, not the applied value.

#### Reciprocal
A Material Property Drawer that sets the inverse of the value entered.

`Reciprocal`

#### OneMinus
A Material Property Drawer that sets one minus the value entered.

`OneMinus`

#### Multiply
A Material Property Drawer that multiplies the value entered.
Works on Float, Range and Int/Integer ShaderLab properties.

`Multiply(float multiplier)`

`Multiply(float multiplier, bool negative)`

### Inline Properties
A Material Property Drawer that sets the inverse of the value entered.

#### MiniThumbnail
Displays a Texture Property on a single line.

`MiniThumbnail`

#### Inline
Displays up to three properties on a single line.
Can also enable a Keyword based on the properties values.

`Inline(string extraProperty1)`

`Inline(string extraProperty1, string extraProperty2)`

`Inline(string extraProperty1, string extraProperty2, string keyword)`

`Inline(string extraProperty1, string extraProperty2, string keyword, bool check1stProperty, bool check2ndProperty, bool check3rdProperty)`

#### SetKeyword
Toggles a Keyword based on the property value.
Not null for textures, above 0 for numeric types.

`SetKeyword(string keyword, string state)`

### Advanced / Specific
Those Material Property Drawers help match Unity's specific Material UI.

#### NormalMap
Can be used on a NormalMap, HeightMap, OcclusionMap.
It will only display the second property if the main map property is not null, and will also enable the keyword if the texture is not null and second property value is greater than zero.

`NormalMap(string strengthProperty, string keyword)`

#### MaterialWorkflow (URP)
To be used on the MetallicMap property.

**Work-In-Progress**

`MaterialWorkflow(string metallicMapScaleProp, string specularMapProp, string specularColorProp, string smoothnessProp, string smoothnessSourceProp)`

