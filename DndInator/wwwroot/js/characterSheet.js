// Character Sheet PDF Filling Module
// Uses pdf-lib to fill form-fillable PDFs

window.characterSheetModule = {
    // Fill a PDF character sheet and return as data URL
    fillCharacterSheet: async function(pdfUrl, characterData) {
        try {
            console.log('Loading PDF-lib...');
            
            // Load pdf-lib from CDN
            const { PDFDocument, rgb } = await this.loadPdfLib();
            
            console.log('Fetching PDF from:', pdfUrl);
            
            // Fetch the PDF
            const existingPdfBytes = await fetch(pdfUrl).then(res => {
                if (!res.ok) throw new Error(`Failed to fetch PDF: ${res.statusText}`);
                return res.arrayBuffer();
            });
            
            console.log('Loading PDF document...');
            
            // Load the PDF
            const pdfDoc = await PDFDocument.load(existingPdfBytes);
            
            // Get the form
            const form = pdfDoc.getForm();
            
            console.log('PDF loaded, filling fields...');
            
            // Fill the form fields based on character data
            this.fillFormFields(form, characterData);
            
            console.log('Saving PDF...');
            
            // Serialize the PDFDocument to bytes
            const pdfBytes = await pdfDoc.save();
            
            // Convert to data URL
            const blob = new Blob([pdfBytes], { type: 'application/pdf' });
            const dataUrl = await this.blobToDataUrl(blob);
            
            console.log('PDF filled successfully');
            
            return dataUrl;
        } catch (error) {
            console.error('Error filling character sheet:', error);
            throw error;
        }
    },
    
    // Load pdf-lib library dynamically
    loadPdfLib: async function() {
        if (window.pdfjsLib) {
            return window.pdfjsLib;
        }
        
        // Load from CDN
        const script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/pdf-lib@1.17.1/dist/pdf-lib.min.js';
        
        return new Promise((resolve, reject) => {
            script.onload = () => {
                if (window.PDFLib) {
                    resolve(window.PDFLib);
                } else {
                    reject(new Error('pdf-lib failed to load'));
                }
            };
            script.onerror = () => reject(new Error('Failed to load pdf-lib script'));
            document.head.appendChild(script);
        });
    },
    
    // Fill form fields with character data
    fillFormFields: function(form, character) {
        try {
            // Get all fields for debugging
            const fields = form.getFields();
            console.log('Available form fields:', fields.map(f => f.getName()));
            
            // Helper function to safely set text field
            const setTextField = (fieldName, value) => {
                try {
                    if (!value) return;
                    const field = form.getTextField(fieldName);
                    field.setText(String(value));
                    console.log(`Set ${fieldName} = ${value}`);
                } catch (e) {
                    console.warn(`Could not set field ${fieldName}:`, e.message);
                }
            };
            
            // Helper function to safely set checkbox
            const setCheckBox = (fieldName, checked) => {
                try {
                    const field = form.getCheckBox(fieldName);
                    if (checked) {
                        field.check();
                    } else {
                        field.uncheck();
                    }
                    console.log(`Set checkbox ${fieldName} = ${checked}`);
                } catch (e) {
                    console.warn(`Could not set checkbox ${fieldName}:`, e.message);
                }
            };
            
            // Calculate ability modifiers
            const calcModifier = (score) => {
                if (!score) return '+0';
                const mod = Math.floor((score - 10) / 2);
                return mod >= 0 ? `+${mod}` : `${mod}`;
            };
            
            // Basic Information
            if (character.information) {
                setTextField('CharacterName', character.information.name);
                setTextField('CharacterName 2', character.information.name);
                setTextField('ClassLevel', character.information.class);
                setTextField('Background', character.information.background);
                setTextField('PlayerName', character.information.playerName);
                setTextField('Race ', character.information.race);
                setTextField('Alignment', character.information.alignment);
                setTextField('XP', character.information.experiencePoints);
            }
            
            // Ability Scores
            if (character.stats) {
                // Strength
                setTextField('STRmod', calcModifier(character.stats.strength));
                setTextField('STR', character.stats.strength);
                
                // Dexterity
                setTextField('DEXmod ', calcModifier(character.stats.dexterity));
                setTextField('DEX', character.stats.dexterity);
                
                // Constitution
                setTextField('CONmod', calcModifier(character.stats.constitution));
                setTextField('CON', character.stats.constitution);
                
                // Intelligence
                setTextField('INTmod', calcModifier(character.stats.intelligence));
                setTextField('INT', character.stats.intelligence);
                
                // Wisdom
                setTextField('WISmod', calcModifier(character.stats.wisdom));
                setTextField('WIS', character.stats.wisdom);
                
                // Charisma
                setTextField('CHamod', calcModifier(character.stats.charisma));
                setTextField('CHA', character.stats.charisma);
            }
            
            // Character Class Details
            if (character.class) {
                setTextField('HPMax', character.class.hitPointDie);
                
                // Proficiency bonus (assume level 1 = +2)
                setTextField('ProfBonus', '+2');
            }
            
            // Background
            if (character.background) {
                setTextField('PersonalityTraits ', character.background.description);
                
                // Skills from background
                if (character.background.skillProficiencies) {
                    const skills = character.background.skillProficiencies.join(', ');
                    setTextField('ProficienciesLang', skills);
                }
            }
            
            // Feats
            if (character.feats && character.feats.length > 0) {
                const featList = character.feats.map(f => f.name).join(', ');
                setTextField('Features and Traits', featList);
            }
            
            // Species/Race traits
            if (character.race) {
                setTextField('Speed', character.race.speed);
                
                if (character.race.traits && character.race.traits.length > 0) {
                    const traits = character.race.traits.join('\n');
                    setTextField('Features and Traits', traits);
                }
            }
            
            console.log('Form filling complete');
        } catch (error) {
            console.error('Error in fillFormFields:', error);
            throw error;
        }
    },
    
    // Convert blob to data URL
    blobToDataUrl: function(blob) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onloadend = () => resolve(reader.result);
            reader.onerror = reject;
            reader.readAsDataURL(blob);
        });
    },
    
    // Get list of all form fields in a PDF (for debugging)
    getFormFields: async function(pdfUrl) {
        try {
            const { PDFDocument } = await this.loadPdfLib();
            
            const existingPdfBytes = await fetch(pdfUrl).then(res => res.arrayBuffer());
            const pdfDoc = await PDFDocument.load(existingPdfBytes);
            const form = pdfDoc.getForm();
            
            const fields = form.getFields();
            return fields.map(field => ({
                name: field.getName(),
                type: field.constructor.name
            }));
        } catch (error) {
            console.error('Error getting form fields:', error);
            throw error;
        }
    },
    
    // Download PDF directly
    downloadPdf: function(dataUrl, filename) {
        const link = document.createElement('a');
        link.href = dataUrl;
        link.download = filename || 'character-sheet.pdf';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
};

console.log('Character Sheet Module loaded');
