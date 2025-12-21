namespace DndShared.Models
{
    public class CharacterEquipment
    {
        public List<AdventuringGear> AdventuringGears { get; set; } = new List<AdventuringGear>();
        public List<Armor> Armors { get; set; } = new List<Armor>();
        public List<Weapon> Weapons { get; set; } = new List<Weapon>();
        public List<MagicItem> MagicItems { get; set; } = new List<MagicItem>();
        public List<MountAndVehicle> MountsAndVehicles { get; set; } = new List<MountAndVehicle>();
        public List<Tool> Tools { get; set; } = new List<Tool>();
        public List<Poison> Poisons { get; set; } = new List<Poison>();
        public List<CharacterCustomItem> CustomItems { get; set; } = new List<CharacterCustomItem>();
    }

    public class CharacterCustomItem
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Weight { get; set; }
    }
}
