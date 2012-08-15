/*

	Make Widget 1.0
	
	Code: Arlo Rose

	This is a Photoshop JavaScript Script that when run will take the open
	document and make a complete Widget from it.
	
	(c) 2003 - 2005 Pixoria

*/

// Get the platform in order to enable or disable the shadow option:
var isMac = (File.fs == "Macintosh") ? true : false;
var isCS2 = (app.version.split(".")[0] > 8) ? true : false;
var nonNormalAlert = false;

// This function creates a dynamic dialog for choosing Widget creation options:
function makeDialog(widgetInfo)
{
	var docRef = activeDocument;
	var dialogW = 390;					// dialog width
	var dialogH = (isMac) ? 352 : 320 ;	// dialog height
	var buttonW = 80;					// button width
	var buttonH = (isCS2) ? 22 : 20;	// button height
	var textW = 270;					// text width
	var textH = 22;						// text height
	var margin = 10;					// margin

	var bounds = {x:0, y:0, width:dialogW, height:dialogH};
	var yLayout = margin;

	var myWindow = new Window("dialog", "Please Choose Your Widget's Options", bounds);

	bounds = {x:(dialogW - buttonW - margin), y:yLayout, width:buttonW, height:buttonH};
	myWindow.buttonRun = myWindow.add("button", bounds, "Create");
	myWindow.defaultElement = myWindow.buttonRun;
	myWindow.buttonRun.onClick = buttonRunOnClick;

	yLayout += buttonH + margin;
	bounds = {x:(dialogW - buttonW - margin), y:yLayout, width:buttonW, height:buttonH};
	myWindow.buttonCancel = myWindow.add("button", bounds, "Cancel");
	myWindow.buttonCancel.onClick = function() { this.parent.close(0); };

	yLayout = margin; // reset

	bounds = {x:margin, y:yLayout, width:180, height:textH};
	myWindow.add("statictext", bounds, "Widget Name:");

	yLayout += textH;
	bounds = {x:margin, y:yLayout, width:textW, height:textH};
	myWindow.widgetName = myWindow.add("edittext", bounds, widgetInfo.theWidgetName);

	yLayout += textH + margin;
	bounds = {x:margin, y:yLayout, width:180, height:textH};
	myWindow.add("statictext", bounds, "Author:");

	yLayout += textH;
	bounds = {x:margin, y:yLayout, width:textW, height:textH};
	myWindow.widgetAuthor = myWindow.add("edittext", bounds, widgetInfo.theWidgetAuthor);

	yLayout += textH + margin;
	bounds = {x:margin, y:yLayout, width:textW/2, height:textH};
	myWindow.add("statictext", bounds, "Version:");

	bounds = {x:(textW/2)+margin, y:yLayout, width:textW/2, height:textH};
	myWindow.add("statictext", bounds, "Minimum K Version:");

	yLayout += textH;
	bounds = {x:margin, y:yLayout, width:(textW/2)-margin, height:textH};
	myWindow.widgetVersion = myWindow.add("edittext", bounds, widgetInfo.widgetVersion);

	bounds = {x:(textW/2)+margin, y:yLayout, width:(textW/2), height:textH};
	myWindow.widgetMinVersion = myWindow.add("edittext", bounds, widgetInfo.minVersion);

	yLayout += textH + margin;
	bounds = {x:margin, y:yLayout, width:180, height:textH};
	myWindow.add("statictext", bounds, "Destination:");

	yLayout += textH;
	bounds = {x:margin, y:yLayout, width:textW-buttonW-margin-5, height:textH}; // 5px smaller
	myWindow.fileDestination = myWindow.add("edittext", bounds, widgetInfo.theSavePath);

	bounds = {x:margin+textW-buttonW-5, y:yLayout, width:buttonW+5, height:buttonH}; // 5px wider
	myWindow.buttonBrowse = myWindow.add("button", bounds, "Browse...");
	myWindow.buttonBrowse.onClick = buttonBrowseOnClick;

	yLayout += textH + margin;
	bounds = {x:margin, y:yLayout, width:textW, height:textH};
	myWindow.checkboxFullWidget = myWindow.add( "checkbox", bounds, "Build Full Widget");
	myWindow.checkboxFullWidget.value = widgetInfo.createFullWidget;

	yLayout += textH + margin;
	bounds = {x:margin, y:yLayout, width:textW, height:textH};
	myWindow.checkboxDebug = myWindow.add( "checkbox", bounds, "Enable Widget's Debug Mode");
	myWindow.checkboxDebug.value = widgetInfo.enableDebug;

	if (isMac)
	{
		yLayout += textH + margin;
		bounds = {x:margin, y:yLayout, width:textW, height:textH};
		myWindow.checkboxShadow = myWindow.add( "checkbox", bounds, "Enable Aqua Drop Shadow");
		myWindow.checkboxShadow.value = widgetInfo.aquaShadow;
	}

	yLayout += textH + margin;
	bounds = {x:margin, y:yLayout, width:textW, height:textH};
	myWindow.checkboxHiddenLayers = myWindow.add( "checkbox", bounds, "Build Widget With Hidden Layers");
	myWindow.checkboxHiddenLayers.value = widgetInfo.addHiddenLayers;


	myWindow.center();
	var result = myWindow.show();
	if ( 0 == result ) return result;  // close to quit

	widgetInfo.theWidgetName = myWindow.widgetName.text;
	widgetInfo.theWidgetAuthor = myWindow.widgetAuthor.text;
	widgetInfo.widgetVersion = myWindow.widgetVersion.text;
	widgetInfo.minVersion = myWindow.widgetMinVersion.text;
	widgetInfo.aquaShadow = (isMac) ? myWindow.checkboxShadow.value : false;
	widgetInfo.enableDebug = myWindow.checkboxDebug.value;
	widgetInfo.createFullWidget = myWindow.checkboxFullWidget.value;
	widgetInfo.theSavePath = myWindow.fileDestination.text;
	widgetInfo.addHiddenLayers = myWindow.checkboxHiddenLayers.value

	instructionData = widgetInfo.widgetVersion + "," + widgetInfo.minVersion + ",";
	instructionData = (widgetInfo.aquaShadow) ? instructionData + "1," : instructionData + ",";
	instructionData = (widgetInfo.createFullWidget) ? instructionData + "1," : instructionData + ",";
	instructionData = (widgetInfo.enableDebug) ? instructionData + "1," : instructionData + ",";
	instructionData = (widgetInfo.addHiddenLayers) ? instructionData + "1" : instructionData + "";

	app.activeDocument.info.instructions = instructionData;
	app.activeDocument.info.title = widgetInfo.theWidgetName;
	app.activeDocument.info.author = widgetInfo.theWidgetAuthor;

	return result;
}

function buttonBrowseOnClick() // call back function
{
	var defaultFolder = this.parent.fileDestination.text;
	var testFolder = new Folder(this.parent.fileDestination.text);
	if (!testFolder.exists) defaultFolder = "";
	var selFolder = Folder.selectDialog("Please choose a destination folder:", defaultFolder);
	if ( selFolder != null ) {
		this.parent.fileDestination.text = selFolder.toString();
	}
	return;
}

function buttonRunOnClick()
{
	this.parent.close(1);
}


// This function is responsible for writing the Header portion of the .kon file
function writeWidgetHeader(myFile, docRefName, widgetInfo)
{
	var docRef = activeDocument;
	myFile.writeln('<?xml version="1.0" encoding="macintosh"?>');
	myFile.writeln('<widget version="' + widgetInfo.widgetVersion + '" minimumVersion="' + widgetInfo.minVersion + '">');
	if (widgetInfo.enableDebug)
	{
		myFile.writeln('<debug>on</debug>');
	}
	else
	{
		myFile.writeln('<debug>off</debug>');
	}
	myFile.writeln('<!--');
	myFile.writeln('	 ' + widgetInfo.theWidgetName);
	if (docRef.info.copyrightNotice != '') myFile.writeln(docRef.info.copyrightNotice);
	if (widgetInfo.theWidgetAuthor !='') myFile.writeln('	 Written by: '+ widgetInfo.theWidgetAuthor);
	if (widgetInfo.theWidgetAuthor !='' || docRef.info.title != '') myFile.writeln('');
	if (docRef.info.caption != '')
	{
		myCaption = docRef.info.caption.split("\n");
	
		for (line in myCaption)
		{
			myFile.writeln('	 ' + myCaption[line]);
		}
		myFile.writeln('');
	}
	myFile.writeln('	 Generated by Photoshop Widget Generator Script');
	myFile.writeln('	 Copyright (C) 2004 - 2005 Pixoria, Inc. All Rights Reserved.');
	myFile.writeln('');
	myFile.writeln('	 Any modifications will be lost if the generation script is run again.');
	myFile.writeln('-->');
	myFile.writeln('');
}

/*
	Parse out any .kon code stored in .psd file ownerUrl text (defined inside File Info...)
*/
function writeElementOwnerURLText(myFile, elementName)
{
	docRef = activeDocument;
	ownerURLText = docRef.info.ownerUrl;
	beginIndex = ownerURLText.indexOf('	<'+ elementName + '>') + elementName.length + 2;
	endIndex = ownerURLText.indexOf('	</'+ elementName + '>');
	for (var i = beginIndex; i < endIndex; i++)
	{
		myFile.write(ownerURLText[i]);
	}
}

/*  This function is for fixing the px that Photoshop 8 sticks in.  */
function fixValue(theValue)
{
	theValue = String(theValue).replace(' px', '');
	return theValue;
}

/*  This function is for removing spaces and other bad characters in layer names.  */
function fixSpaces(theValue)
{
	// Photoshop seems to have an incomplete RegEx engine so this is a workaround:
	while (theValue.match(/\W/))
	{
		theValue = String(theValue).replace(/\W/, '_');
	}
	return theValue;
}



// This function does most of the hard work and is responsible for extracting the layer information
// contained within the Photoshop document and building the representations of them in the generated
// .kon file.  It also merges and extracts the contents of each art layer into a separate .png file.
// If it encounters a text layer, it will ask whether to rasterize it or keep it as plain text.
function writeLayerDefinitions(myFile, resource_dir, widgetInfo)
{
	var origVisibilities = [];
	var docRef = activeDocument;

	// Save original document state so that we can return to it when done
	var originalDocumentState = docRef.activeHistoryState;

	// Delete hidden layers
	numLayers = docRef.artLayers.length;
	do
	{
		numLayers = docRef.artLayers.length;
		mustRestart = false;
		for (var i = 0; i < numLayers; i++)
		{
			if ( docRef.layers[i].visible == false )
			{
				docRef.layers[i].remove();
				mustRestart = true;
				break;
			}
		}
	}
	while (mustRestart);
	// Merge any grouped or embedded art layers in top down order	
	do
	{
		numLayers = docRef.artLayers.length;
		mustRestart = false;
		for (var i = numLayers-1; i >= 0; i--)
		{
			if (docRef.artLayers[i].grouped)
			{
				docRef.artLayers[i].merge();
				mustRestart = true;
				break;
			}
		}
	
	}
	while (mustRestart);

	// Now process remaining layers
	numLayers = docRef.layers.length;

	// Save original layer visibilities (not needed any longer)
	for (var i = 0; i < numLayers; i++)
	{
		origVisibilities[i] = docRef.layers[i].visible;
		docRef.layers[i].visible = false;
	}
		
	// Scan layers and extract information to build representation in .kon file
	// Also, extract each individual layer and save as a separate .png file
	for (var i = (numLayers-1); i >= 0; i--)
	{	
		if (docRef.layers[i].blendMode != "BlendMode.NORMAL")
		{
			if (!nonNormalAlert) alert ( "This file contains layers that use a blend mode other than \"Normal\".\nBe aware that as a Widget, the layers that don't use \"Normal\" may not blend as you'd expect." );
			nonNormalAlert = true;
		}
		if ( origVisibilities[i] == true || widgetInfo.addHiddenLayers == true)
		{
			// Ask user if they would like to have the text layer rasterized
			var rasterizeText = true;
			if (docRef.layers[i].kind == LayerKind.TEXT)
			{
				rasterizeText = confirm('Make text layer "'+ docRef.layers[i].name +'" into an image?');
			}
		
			// If 'false' and is a Text layer, treat the text as a Konfabulator <text> object.
			// Extract position, font, size, alignment, etc. for building representation in .kon file
			if (!rasterizeText && (docRef.layers[i].kind == LayerKind.TEXT))
			{
				if (docRef.layers[i].textItem.kind == TextType.POINTTEXT)
				{
					myFile.writeln('	<text data="' + docRef.layers[i].textItem.contents + '">');
					myFile.writeln('		<name>'+ fixSpaces(docRef.layers[i].name) +'</name>');
					var pos = docRef.layers[i].textItem.position;
					myFile.writeln('		<hOffset>'+ fixValue(pos[0]) +'</hOffset>');
					myFile.writeln('		<vOffset>'+ fixValue(pos[1]) +'</vOffset>');
					if (docRef.layers[i].textItem.justification == Justification.CENTER)
					{
						align = 'center';
					}
					else if (docRef.layers[i].textItem.justification == Justification.RIGHT)
					{
						align = 'right';
					}
					else
					{
						align = 'left';
					}
					myFile.writeln('		<alignment>'+ align +'</alignment>');
					myFile.writeln('		<font>'+ fonts[docRef.layers[i].textItem.font].name +'</font>');
					myFile.writeln('		<size>'+ docRef.layers[i].textItem.size +'</size>');
					myFile.writeln('		<color>#'+ docRef.layers[i].textItem.color.rgb.hexValue +'</color>');
					if (origVisibilities[i] == false)
					{
						myFile.writeln('		<opacity>0%</opacity>');
					}
					else
					{
						myFile.writeln('		<opacity>'+ Math.floor(docRef.layers[i].opacity)+'%</opacity>');
					}
					writeElementOwnerURLText(myFile, docRef.layers[i].name);
					myFile.writeln('	</text>');
				}
				else
				{ // TextType.PARAGRAPHTEXT
					myFile.writeln('	<textarea data="' + docRef.layers[i].textItem.contents + '">');
					myFile.writeln('		<name>'+ fixSpaces(docRef.layers[i].name) +'</name>');
					var pos = docRef.layers[i].textItem.position;
					myFile.writeln('		<hOffset>'+ fixValue(pos[0]) +'</hOffset>');
					myFile.writeln('		<vOffset>'+ fixValue(pos[1]) +'</vOffset>');
					myFile.writeln('		<width>'+ fixValue(docRef.layers[i].textItem.width) +'</width>');
					myFile.writeln('		<height>'+ fixValue(docRef.layers[i].textItem.height) +'</height>');
					if (docRef.layers[i].textItem.justification == Justification.CENTER)
					{
						align = 'center';
					}
					else if (docRef.layers[i].textItem.justification == Justification.RIGHT)
					{
						align = 'right';
					}
					else
					{
						align = 'left';
					}
					myFile.writeln('		<alignment>'+ align +'</alignment>');
					myFile.writeln('		<!-- If the font does not render as expected, -->');
					myFile.writeln('		<!-- verify the font name below is correct.   -->');
					myFile.writeln('		<font>'+ docRef.layers[i].textItem.font+'</font>');
					myFile.writeln('		<size>'+ docRef.layers[i].textItem.size +'</size>');
					myFile.writeln('		<color>#'+ docRef.layers[i].textItem.color.rgb.hexValue +'</color>');
					if (origVisibilities[i] == false)
					{
						myFile.writeln('		<opacity>0%</opacity>');
					}
					else
					{
						myFile.writeln('		<opacity>'+ Math.floor(docRef.layers[i].opacity)+'%</opacity>');
					}
					writeElementOwnerURLText(myFile, docRef.layers[i].name);
					myFile.writeln('	</textarea>');
				}
			
			}
			else
			{ // Treat layer as an image layer

				// Make current layer visible
				docRef.layers[i].visible = true;
			
				// Cache name (I've hit a strange problem where "Background" can becaome "Layer 0"
				theLayerName = docRef.layers[i].name;

				// Save original width and height of document to compute hOffset and vOffset of layer
				beforeWidth = docRef.width;
				beforeHeight = docRef.height;
			
				// Save current document state as a restore point
				savedState = docRef.activeHistoryState;
			
				// Trim away all transparent pixels to the left and above of layer content
				if (theLayerName != "Background")
				{
					docRef.trim(TrimType.TRANSPARENT, true, true, false, false);
				}
			
				// hOffset and vOffset is the original dimensions minus the resulting dimensions
				hOffset = beforeWidth - (docRef.width * 1);
				vOffset = beforeHeight - (docRef.height * 1);
			
				// Trim the bottom and right portions to: a) get width and height info, and b) to save as a .png
				if (theLayerName != "Background")
				{
					docRef.trim(TrimType.TRANSPARENT, false, false, true, true);
				}

				// Extract layer details for .kon representation
				myFile.writeln('	<image src="Resources/' + theLayerName +'.png">');
				myFile.writeln('		<name>'+ fixSpaces(theLayerName) +'</name>');
				myFile.writeln('		<hOffset>' + fixValue(hOffset) + '</hOffset>');
				myFile.writeln('		<vOffset>' + fixValue(vOffset) + '</vOffset>');
				myFile.writeln('		<width>'+ fixValue((docRef.width * 1)) +'</width>');
				myFile.writeln('		<height>'+ fixValue((docRef.height * 1)) +'</height>');
				if (origVisibilities[i] == false)
				{
					myFile.writeln('		<opacity>0%</opacity>');
				}
				else
				{
					myFile.writeln('		<opacity>'+ Math.floor(docRef.layers[i].opacity)+'%</opacity>');
				}
				writeElementOwnerURLText(myFile, theLayerName);
				myFile.writeln('	</image>');
			
				// Set to full opacity before extracting, otherwise we would be essentially doubling the opacity
				// (once during rendering in Photoshop, and again when Konfabulator interprets the opacity attribute)
				docRef.layers[i].opacity = 100;
			
				// Extract layer into separate .png file
				if (widgetInfo.createFullWidget)
				{
					pngFile = new File(resource_dir +'/'+ theLayerName +'.png');
				}
				else 
				{
					pngFile = new File(widgetInfo.theSavePath +'/' + widgetInfo.theWidgetName + '.widget/Contents/Resources/'+ theLayerName +'.png');
				}
				pngSaveOptions = new PNGSaveOptions();
				pngSaveOptions.interlaced = false;
				pngSaveOptions.typename = SaveDocumentType.PNG;
				docRef.saveAs(pngFile, pngSaveOptions, true, Extension.LOWERCASE);
			
				// Restore to pre-Trim state
				docRef.activeHistoryState = savedState;
			
				// Clean up
				delete pngFile;
				delete pngSaveOptions;
				delete savedState;
			
				// Done working on this layer, hide it
				docRef.layers[i].visible = false;
			}
		
			myFile.writeln('');
		}
	}

	// Restore original document state
	docRef.activeHistoryState = originalDocumentState;

	// Restore original document layer visibilities
	for (i = 0; i < numLayers; i++)
	{
		docRef.layers[i].visible = origVisibilities[i];
	}
}

// This function is responsible for writing out the <window> block of the .kon file.
// The .kon file representation is based on the current Photoshop active document values.
function writeWindowDefinition(myFile, docRefName, widgetInfo)
{
	var docRef = activeDocument;

	myFile.writeln('	<window title="' + widgetInfo.theWidgetName + '">');
	myFile.writeln('		<name>mainWindow</name>');
	myFile.writeln('		<width>'+ fixValue((docRef.width * 1)) +'</width>');
	myFile.writeln('		<height>'+ fixValue((docRef.height * 1))+'</height>');
	myFile.writeln('		<visible>1</visible>');
	if (widgetInfo.aquaShadow)
	{
		myFile.writeln('		<shadow>1</shadow>');
	}
	else
	{
		myFile.writeln('		<shadow>0</shadow>');
	}
	myFile.writeln('	</window>');
	myFile.writeln('');
}

// This function is responsible for finishing up the .kon file.  Currently, all that is
// needed is to close up the widget definition.
function writeWidgetFooter(myFile)
{
	myFile.writeln('</widget>');
}

// This function write out the overall layout of the widget, the window representation
// and the layer representations.
function writeWidgetLayout(myFile, resource_dir, docRefName, widgetInfo)
{
	writeWindowDefinition(myFile, docRefName, widgetInfo);
	writeLayerDefinitions(myFile, resource_dir, widgetInfo);
}

// This is the main function used to begin code generation for the widget.
function createWidget(widgetInfo)
{


	var safeDocRef = activeDocument;
	var docRef = safeDocRef.duplicate();

	// Get document name and remove '.psd' extension (if present) from generated name
	var docRefName = safeDocRef.name;
	docRefName = docRefName.replace('.psd', '');

	// Remember original ruler Unit preference
	var origUnit = preferences.rulerUnits;

	// Set rulerUnits to PIXELS for the duration of this script.
	preferences.rulerUnits = Units.PIXELS;

	if (widgetInfo.createFullWidget)
	{
		var widget_dir = widgetInfo.theSavePath +'/' + widgetInfo.theWidgetName + '.widget';
		widgetDir = new Folder(widget_dir);
		widgetDir.create();
	
		var contents_dir = widgetInfo.theSavePath +'/' + widgetInfo.theWidgetName + '.widget/Contents';
		contentsDir = new Folder(contents_dir);
		contentsDir.create();

		var resource_dir = widgetInfo.theSavePath +'/' + widgetInfo.theWidgetName + '.widget/Contents/Resources';
		resDir = new Folder(resource_dir);
		resDir.create();
	
		// Backup existing .kon file if present.
		// Using a .txt extension instead of .kon to avoid confusion.
		var origName = widgetInfo.theSavePath +'/' + widgetInfo.theWidgetName + '.widget/Contents/'+ widgetInfo.theWidgetName +'.kon';
		var backName = widgetInfo.theSavePath +'/' + widgetInfo.theWidgetName + '.widget/Contents/'+ widgetInfo.theWidgetName +' backup.txt';
		origFile = new File(origName);
		if (origFile.open('r', 'TEXT','K0nF'))
		{
			backFile = new File(backName);
			backFile.open('w', 'TEXT','R*ch');
			while (!origFile.eof)
			{
				line = origFile.readln();
				backFile.writeln(line);
			}
			origFile.close();
			backFile.close();
		}
	
		// Open .kon file to hold generated code
		var filename = widgetInfo.theSavePath +'/' + widgetInfo.theWidgetName + '.widget/Contents/'+ widgetInfo.theWidgetName +'.kon';
	
	} else {

		// Create Resources folder (if none exists) to hold rendered images
		var resource_dir = widgetInfo.theSavePath +'/Resources';
		resDir = new Folder(resource_dir);
		resDir.create();
	
		// Backup existing .kon file if present.
		// Using a .txt extension instead of .kon to avoid confusion.
		var origName = widgetInfo.theSavePath +'/'+ docRefName +'.kon';
		var backName = widgetInfo.theSavePath +'/'+ docRefName +' backup.txt';
		origFile = new File(origName);
		if (origFile.open('r', 'TEXT','K0nF'))
		{
			backFile = new File(backName);
			backFile.open('w', 'TEXT','R*ch');
			while (!origFile.eof)
			{
				line = origFile.readln();
				backFile.writeln(line);
			}
			origFile.close();
			backFile.close();
		}
	
		// Open .kon file to hold generated code
		var filename = widgetInfo.theSavePath +'/'+ docRefName +'.kon';
	}

	myFile = new File(filename);
	myFile.open('w', 'TEXT','K0nF');

	// Write out data...
	writeWidgetHeader(myFile, docRefName, widgetInfo);
	writeWidgetLayout(myFile, resource_dir, docRefName, widgetInfo);
	writeElementOwnerURLText(myFile, docRefName, widgetInfo);
	writeWidgetFooter(myFile);

	// Close it up
	myFile.close();

	// Clean up and return things to original settings
	docRef.trim(TrimType.TRANSPARENT, true, true, true, true);
	preferences.rulerUnits = origUnit;
	docRef.close(SaveOptions.DONOTSAVECHANGES);
}

function setUpDialogInfo(widgetInfo)
{
	if (app.activeDocument.info.instructions == "")
	{
		var instructionData = "1.0,2.0,,1,1,";
		app.activeDocument.info.instructions = instructionData;
	}
	else
	{
		var instructionData = app.activeDocument.info.instructions;
	}

	instructionData = instructionData.split(",");

	widgetInfo.widgetVersion = instructionData[0];
	widgetInfo.minVersion = instructionData[1];
	widgetInfo.aquaShadow = instructionData[2];
	widgetInfo.createFullWidget = instructionData[3];
	widgetInfo.enableDebug = instructionData[4];
	widgetInfo.addHiddenLayers = instructionData[5];

	if (app.activeDocument.info.title)
	{
		widgetInfo.theWidgetName = app.activeDocument.info.title;
	}
	else
	{
		widgetInfo.theWidgetName = app.activeDocument.name.replace('.psd', '');
	}

	if (app.activeDocument.info.author)
	{
		widgetInfo.theWidgetAuthor = app.activeDocument.info.author;
	}
	else
	{
		widgetInfo.theWidgetAuthor = "";
	}

	try {
		widgetInfo.theSavePath = String(app.activeDocument.path);
	} catch(someError) {
		if (isMac)
		{
			widgetInfo.theSavePath = "~/Desktop";
		}
		else
		{
			widgetInfo.theSavePath = "";
		}
	}
}

function main()
{
	if ( app.documents.length <= 0 ) {
		alert( "No Open Documents\nYou must have a document open to create a Widget from." );
		return;
	}
	var widgetInfo = new Object();
	setUpDialogInfo(widgetInfo);
	if (0 == makeDialog(widgetInfo)) return;
	var widgetName = widgetInfo.theWidgetName;
	if (widgetName.match(/[\!|\?]/) && !isMac) {
		alert ( "Bad File Name\nThe file name you're using contains characters that will not work on Windows." );
		main();
		return;
	}
	var destination = widgetInfo.theSavePath;
	if (destination.length == 0) {
		alert("Please Specify Destination\nIn order to save your Widget, you need to specify a destination folder for it.");
		main();
		return;
	}
	createWidget(widgetInfo);
	alert("Creation Is Complete\nA Widget has been created from your documnet and is ready to go!");
}


main();