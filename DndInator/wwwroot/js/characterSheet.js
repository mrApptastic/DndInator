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
            
            // // Flatten the form to convert fields to static content
            // // This ensures the portrait will be drawn on top
            // console.log('Flattening form fields...');
            // form.flatten();
            
            // // Add character portrait if available
            // // Must be done AFTER flattening so it appears on top
            // if (characterData.characterPortrait) {
            //     console.log('Adding character portrait...');
            //     await this.addPortraitImage(pdfDoc, characterData.characterPortrait);
            //     // await this.drawPortraitOnCanvas('canvas.annotationContent', characterData.characterPortrait);    
            // }
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
                    if (value === null || value === undefined) return;
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
            
            // Calculate proficiency bonus based on level
            const calcProficiencyBonus = (level) => {
                if (!level || level < 1) return '+2';
                const bonus = Math.ceil(level / 4) + 1;
                return `+${bonus}`;
            };
            
            // ===== PAGE 1: CHARACTER INFO & STATS =====
            
            // Basic Information - support both old and new structure
            const baseInfo = character.baseInformation || character.information;
            if (baseInfo) {
                setTextField('Name', baseInfo.characterName || baseInfo.name);
                setTextField('Level', baseInfo.level);
                setTextField('XP Points', baseInfo.experiencePoints);
                setTextField('PERSONALITY', baseInfo.description || baseInfo.personalityTraits);
            }
            
            // Background
            if (character.background) {
                setTextField('Background', character.background.name);
            }
            
            // Class
            if (character.class) {
                setTextField('Class', character.class.name);
                setTextField('Subclass', character.class.subclassName || '');
                
                // Proficiency bonus - use level if available
                const level = baseInfo?.level || 1;
                setTextField('PROF BONUS', calcProficiencyBonus(level));
            }
            
            // Species/Race - support both names
            const species = character.species || character.race;
            if (species) {
                setTextField('Species', species.name);
                setTextField('SPEED', species.speed);
                setTextField('SIZE', species.size);
                
                // Species traits
                if (species.traits && species.traits.length > 0) {
                    const traits = Array.isArray(species.traits[0]) 
                        ? species.traits.map(t => `${t.name}: ${t.description}`).join('\n\n')
                        : species.traits.join('\n');
                    setTextField('SPECIES TRAITS', traits);
                }
            }
            
            // Ability Scores and Modifiers - support both names
            const abilities = character.abilityScores || character.stats;
            if (abilities) {
                // Strength
                const strMod = calcModifier(abilities.strength);
                const strModNum = Math.floor((abilities.strength - 10) / 2);
                setTextField('STR MOD', strMod);
                setTextField('STR SCORE', String(abilities.strength));
                
                // Dexterity
                const dexMod = calcModifier(abilities.dexterity);
                const dexModNum = Math.floor((abilities.dexterity - 10) / 2);
                setTextField('DEX MOD', dexMod);
                setTextField('DEX SCORE', String(abilities.dexterity));
                
                // Constitution
                const conMod = calcModifier(abilities.constitution);
                const conModNum = Math.floor((abilities.constitution - 10) / 2);
                setTextField('CON MOD', conMod);
                setTextField('CON SCORE', String(abilities.constitution));
                
                // Intelligence
                const intMod = calcModifier(abilities.intelligence);
                const intModNum = Math.floor((abilities.intelligence - 10) / 2);
                setTextField('INT MOD', intMod);
                setTextField('INT SCORE', String(abilities.intelligence));
                
                // Wisdom
                const wisMod = calcModifier(abilities.wisdom);
                const wisModNum = Math.floor((abilities.wisdom - 10) / 2);
                setTextField('WIS MOD', wisMod);
                setTextField('WIS SCORE', String(abilities.wisdom));
                
                // Charisma
                const chaMod = calcModifier(abilities.charisma);
                const chaModNum = Math.floor((abilities.charisma - 10) / 2);
                setTextField('CHA MOD', chaMod);
                setTextField('CHA SCORE', String(abilities.charisma));
                
                // Initiative (use Dex modifier)
                setTextField('init', dexMod);
                
                // Passive Perception (10 + Wisdom modifier + proficiency if proficient)
                const passivePerception = 10 + wisModNum;
                setTextField('PASSIVE PERCEPTION', String(passivePerception));
            }
            
            // Skills
            if (character.skills) {
                const level = baseInfo?.level || 1;
                const profBonus = parseInt(calcProficiencyBonus(level).substring(1));
                const stats = abilities || character.stats || {};
                
                // Skill mapping: skill name -> {ability, fieldName, checkboxName}
                const skillMap = {
                    'Acrobatics': { ability: 'dexterity', field: 'ACROBATICS', checkbox: 'Check Box11' },
                    'Animal Handling': { ability: 'wisdom', field: 'ANIMAL HANDLING', checkbox: 'Check Box15' },
                    'Arcana': { ability: 'intelligence', field: 'ARCANA', checkbox: 'Check Box24' },
                    'Athletics': { ability: 'strength', field: 'ATHLETICS', checkbox: 'Check Box19' },
                    'Deception': { ability: 'charisma', field: 'DECEPTION', checkbox: 'Check Box5' },
                    'History': { ability: 'intelligence', field: 'HISTORY', checkbox: 'Check Box20' },
                    'Insight': { ability: 'wisdom', field: 'INSIGHT', checkbox: 'Check Box13' },
                    'Intimidation': { ability: 'charisma', field: 'INTIMIDATE', checkbox: 'Check Box4' },
                    'Investigation': { ability: 'intelligence', field: 'INVESTIGATION', checkbox: 'Check Box21' },
                    'Medicine': { ability: 'wisdom', field: 'MEDICINE', checkbox: 'Check Box12' },
                    'Nature': { ability: 'intelligence', field: 'NATURE', checkbox: 'Check Box22' },
                    'Perception': { ability: 'wisdom', field: 'PERCEPTION', checkbox: 'Check Box14' },
                    'Performance': { ability: 'charisma', field: 'PERFORMANCE', checkbox: 'Check Box3' },
                    'Persuasion': { ability: 'charisma', field: 'PERSUASION', checkbox: 'Check Box2' },
                    'Religion': { ability: 'intelligence', field: 'RELIGION', checkbox: 'Check Box23' },
                    'Sleight of Hand': { ability: 'dexterity', field: 'SLEIGHT OF HAND', checkbox: 'Check Box8' },
                    'Stealth': { ability: 'dexterity', field: 'STEALTH', checkbox: 'Check Box9' },
                    'Survival': { ability: 'wisdom', field: 'SURVIVAL', checkbox: 'Check Box16' }
                };
                
                // Fill each skill
                Object.keys(skillMap).forEach(skillName => {
                    const skillInfo = skillMap[skillName];
                    const abilityScore = stats[skillInfo.ability] || 10;
                    const abilityMod = Math.floor((abilityScore - 10) / 2);
                    const isProficient = character.skills[skillName] === true;
                    
                    const skillBonus = isProficient ? abilityMod + profBonus : abilityMod;
                    const skillBonusStr = skillBonus >= 0 ? `+${skillBonus}` : `${skillBonus}`;
                    
                    setTextField(skillInfo.field, skillBonusStr);
                    setCheckBox(skillInfo.checkbox, isProficient);
                });
            }
            
            // Saving Throws
            if (abilities && character.class) {
                const level = baseInfo?.level || 1;
                const profBonus = parseInt(calcProficiencyBonus(level).substring(1));
                
                // Determine which saves are proficient based on class
                const saveProfString = character.class.savingThrowProficiencies || '';
                const saveProficiencies = Array.isArray(saveProfString) 
                    ? saveProfString 
                    : saveProfString.split(',').map(s => s.trim());
                
                const saves = {
                    'Strength': { score: abilities.strength, field: 'STR SAVE', checkbox: 'Check Box18' },
                    'Dexterity': { score: abilities.dexterity, field: 'DEX SAVE', checkbox: 'Check Box11' },
                    'Constitution': { score: abilities.constitution, field: 'CON SAVE', checkbox: 'Check Box7' },
                    'Intelligence': { score: abilities.intelligence, field: 'INT SAVE', checkbox: 'Check Box25' },
                    'Wisdom': { score: abilities.wisdom, field: 'WIS SAVE', checkbox: 'Check Box17' },
                    'Charisma': { score: abilities.charisma, field: 'CHA SAVE', checkbox: 'Check Box6' }
                };
                
                Object.keys(saves).forEach(saveName => {
                    const saveInfo = saves[saveName];
                    const abilityMod = Math.floor((saveInfo.score - 10) / 2);
                    const isProficient = saveProficiencies.includes(saveName);
                    
                    const saveBonus = isProficient ? abilityMod + profBonus : abilityMod;
                    const saveBonusStr = saveBonus >= 0 ? `+${saveBonus}` : `${saveBonus}`;
                    
                    setTextField(saveInfo.field, saveBonusStr);
                    setCheckBox(saveInfo.checkbox, isProficient);
                });
            }
            
            // Combat Stats
            if (character.equipment || character.combat) {
                // Armor Class (from armor)
                const armor = character.equipment?.armor;
                if (armor) {
                    setTextField('Armor Class', armor.armorClass || '10');
                    setCheckBox('shield chk', armor.isShield || false);
                }
                
                // Hit Points
                const combat = character.combat || {};
                setTextField('Max HP', combat.maxHP || '');
                setTextField('Current HP', combat.currentHP || '');
                setTextField('Temp HP', combat.tempHP || '');
                
                // Hit Dice
                setTextField('Max HD', combat.maxHitDice || '');
                setTextField('Spent HD', combat.spentHitDice || '');
            }
            
            // Weapons
            if (character.equipment?.weapons && character.equipment.weapons.length > 0) {
                for (let i = 0; i < Math.min(6, character.equipment.weapons.length); i++) {
                    const weapon = character.equipment.weapons[i];
                    const weaponNum = i + 1;
                    
                    setTextField(`NAME - WEAPON ${weaponNum}`, weapon.name);
                    setTextField(`BONUS/DC - WEAPON ${weaponNum}`, weapon.attackBonus || '');
                    setTextField(`DAMAGE/TYPE - WEAPON ${weaponNum}`, weapon.damage || '');
                    setTextField(`NOTES - WEAPON ${weaponNum}`, weapon.properties || '');
                }
            }
            
            // Feats
            if (character.feats && character.feats.length > 0) {
                const featText = character.feats.map(f => `${f.name}\n${f.description}`).join('\n\n');
                setTextField('FEATS', featText);
            }
            
            // Class Features
            if (character.class?.features) {
                const featuresText = Array.isArray(character.class.features)
                    ? character.class.features.join('\n\n')
                    : character.class.features;
                setTextField('CLASS FEATURES 1', featuresText);
            }
            
            // Weapon Proficiencies
            if (character.proficiencies?.weapons) {
                setTextField('WEAPON PROF', character.proficiencies.weapons.join(', '));
            } else if (character.background?.equipment) {
                // Fallback to background equipment
                setTextField('WEAPON PROF', character.background.equipment);
            }
            
            // Tool Proficiencies
            if (character.proficiencies?.tools) {
                setTextField('TOOL PROF', character.proficiencies.tools.join(', '));
            } else if (character.background?.toolProficiency) {
                setTextField('TOOL PROF', character.background.toolProficiency);
            }
            
            // ===== PAGE 2: SPELLS & EQUIPMENT =====
            
            // Spellcasting
            if (character.spellcasting) {
                setTextField('SPELLCASTING ABILITY', character.spellcasting.ability || '');
                setTextField('SPELLCASTING MOD', character.spellcasting.modifier || '');
                setTextField('SPELL SAVE DC', character.spellcasting.saveDC || '');
                setTextField('SPELL ATTACK BONUS', character.spellcasting.attackBonus || '');
                
                // Spell slots
                for (let level = 1; level <= 9; level++) {
                    const slots = character.spellcasting[`level${level}Slots`];
                    if (slots !== undefined) {
                        setTextField(`LVL${level} TOTAL`, slots.toString());
                    }
                }
                
                // Spells
                if (character.spellcasting.spells && character.spellcasting.spells.length > 0) {
                    for (let i = 0; i < Math.min(29, character.spellcasting.spells.length); i++) {
                        const spell = character.spellcasting.spells[i];
                        
                        setTextField(`SPELL LEVEL${i}`, spell.level !== undefined ? spell.level.toString() : '');
                        setTextField(`SPELL NAME${i}`, spell.name);
                        setTextField(`CASTING TIME${i}`, spell.castingTime || '');
                        setTextField(`RANGE${i}`, spell.range || '');
                        
                        // Spell components (V, S, M checkboxes)
                        if (spell.components) {
                            const vChecked = spell.components.includes('V') || spell.components.includes('v');
                            const sChecked = spell.components.includes('S') || spell.components.includes('s');
                            const mChecked = spell.components.includes('M') || spell.components.includes('m');
                            
                            // Try different checkbox naming patterns
                            setCheckBox(`Check Box${i * 3}`, vChecked);
                            setCheckBox(`Check Box${i * 3 + 1}`, sChecked);
                            setCheckBox(`Check Box${i * 3 + 2}`, mChecked);
                        }
                        
                        setTextField(`SPELL NOTES${i}`, spell.description || spell.notes || '');
                    }
                }
            }
            
            // Languages
            if (character.languages && character.languages.length > 0) {
                setTextField('LANGUAGES', character.languages.join(', '));
            }
            
            // Equipment
            if (character.equipment) {
                // General equipment list
                const equipmentList = [];
                
                if (character.equipment.armor) {
                    equipmentList.push(character.equipment.armor.name);
                }
                
                if (character.equipment.weapons) {
                    equipmentList.push(...character.equipment.weapons.map(w => w.name));
                }
                
                if (character.equipment.adventuringGear) {
                    equipmentList.push(...character.equipment.adventuringGear.map(g => g.name));
                }
                
                if (character.equipment.tools) {
                    equipmentList.push(...character.equipment.tools.map(t => t.name));
                }
                
                if (equipmentList.length > 0) {
                    setTextField('EQUIPMENT', equipmentList.join('\n'));
                }
            }
            
            // Attunement items (magic items)
            if (character.equipment?.magicItems) {
                for (let i = 0; i < Math.min(3, character.equipment.magicItems.length); i++) {
                    const item = character.equipment.magicItems[i];
                    setTextField(`ATTUNMENT ${i + 1}`, item.name);
                    setCheckBox(`Check Box${61 + i}`, item.requiresAttunement || false);
                }
            }
            
            // Wealth (currency)
            if (character.wealth) {
                setTextField('CP', character.wealth.copper || '0');
                setTextField('SP', character.wealth.silver || '0');
                setTextField('EP', character.wealth.electrum || '0');
                setTextField('GP', character.wealth.gold || '0');
                setTextField('PP', character.wealth.platinum || '0');
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
    
    // Add portrait image to PDF
    addPortraitImage: async function(pdfDoc, imageDataUrl) {
        try {
            // Get the second page (where the portrait box is)
            const pages = pdfDoc.getPages();
            const secondPage = pages[1];
            
            // Parse the data URL to get the image type and data
            const matches = imageDataUrl.match(/^data:image\/(png|jpeg|jpg);base64,(.+)$/);
            if (!matches) {
                console.warn('Invalid image data URL format');
                return;
            }
            
            const imageType = matches[1];
            const imageBase64 = matches[2];
            
            // Convert base64 to bytes
            const imageBytes = Uint8Array.from(atob(imageBase64), c => c.charCodeAt(0));
            
            // Embed the image
            let image;
            if (imageType === 'png') {
                image = await pdfDoc.embedPng(imageBytes);
            } else if (imageType === 'jpeg' || imageType === 'jpg') {
                image = await pdfDoc.embedJpg(imageBytes);
            } else {
                console.warn('Unsupported image type:', imageType);
                return;
            }
            
            // Portrait box dimensions and position (based on the annotation data provided)
            // Position is relative to bottom-left corner of page
            const pageHeight = secondPage.getHeight();
            const pageWidth = secondPage.getWidth();
            
            // From the annotation: left: 68.1435%, top: 17.7558%, width: 29.5358%, height: 21.414%
            // Convert percentages to actual dimensions
            const portraitX = pageWidth * 0.681435;
            const portraitY = pageHeight * (1 - 0.177558 - 0.21414); // Bottom-left origin, so we subtract from top
            const portraitWidth = pageWidth * 0.295358;
            const portraitHeight = pageHeight * 0.21414;
            
            // Scale the image to fit within the portrait box while maintaining aspect ratio
            const imgAspectRatio = image.width / image.height;
            const boxAspectRatio = portraitWidth / portraitHeight;
            
            let drawWidth = portraitWidth;
            let drawHeight = portraitHeight;
            let drawX = portraitX;
            let drawY = portraitY;
            
            if (imgAspectRatio > boxAspectRatio) {
                // Image is wider than box
                drawHeight = portraitWidth / imgAspectRatio;
                drawY = portraitY + (portraitHeight - drawHeight) / 2;
            } else {
                // Image is taller than box
                drawWidth = portraitHeight * imgAspectRatio;
                drawX = portraitX + (portraitWidth - drawWidth) / 2;
            }
            
            // Draw the image on the page
            secondPage.drawImage(image, {
                x: drawX,
                y: drawY,
                width: drawWidth,
                height: drawHeight
            });
            
            console.log('Portrait image added successfully');
        } catch (error) {
            console.error('Error adding portrait image:', error);
            // Don't throw - continue with PDF generation even if portrait fails
        }
    },
    
    // Alternative: Draw portrait directly on canvas after PDF is rendered
    drawPortraitOnCanvas: function(canvasElement, imageDataUrl, pageNumber = 2) {
        return new Promise((resolve, reject) => {
            try {
                const canvas = typeof canvasElement === 'string' 
                    ? document.querySelector(canvasElement) 
                    : canvasElement;
                
                if (!canvas) {
                    console.warn('Canvas element not found');
                    reject(new Error('Canvas not found'));
                    return;
                }
                
                const ctx = canvas.getContext('2d');
                
                // Create image element
                const img = new Image();
                img.onload = function() {
                    // Portrait box dimensions (as percentages of canvas size)
                    const portraitLeft = 0.681435;
                    const portraitTop = 0.177558;
                    const portraitWidthPercent = 0.295358;
                    const portraitHeightPercent = 0.21414;
                    
                    // Convert to actual canvas coordinates
                    const portraitX = canvas.width * portraitLeft;
                    const portraitY = canvas.height * portraitTop;
                    const portraitWidth = canvas.width * portraitWidthPercent;
                    const portraitHeight = canvas.height * portraitHeightPercent;
                    
                    // Scale image to fit while maintaining aspect ratio
                    const imgAspectRatio = img.width / img.height;
                    const boxAspectRatio = portraitWidth / portraitHeight;
                    
                    let drawWidth = portraitWidth;
                    let drawHeight = portraitHeight;
                    let drawX = portraitX;
                    let drawY = portraitY;
                    
                    if (imgAspectRatio > boxAspectRatio) {
                        // Image is wider than box
                        drawHeight = portraitWidth / imgAspectRatio;
                        drawY = portraitY + (portraitHeight - drawHeight) / 2;
                    } else {
                        // Image is taller than box
                        drawWidth = portraitHeight * imgAspectRatio;
                        drawX = portraitX + (portraitWidth - drawWidth) / 2;
                    }
                    
                    // Draw the image on canvas
                    ctx.drawImage(img, drawX, drawY, drawWidth, drawHeight);
                    
                    console.log('Portrait drawn on canvas successfully');
                    resolve();
                };
                
                img.onerror = function() {
                    console.error('Failed to load portrait image');
                    reject(new Error('Failed to load image'));
                };
                
                img.src = imageDataUrl;
            } catch (error) {
                console.error('Error drawing portrait on canvas:', error);
                reject(error);
            }
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
