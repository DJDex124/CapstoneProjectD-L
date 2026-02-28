using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Core inventory data manager. Attach to the Player or a persistent GameManager.
/// Holds all slot data and fires events when things change.
/// No UI code lives here.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory Settings")]
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 8;

    

    // All inventory slots — hotbar is always slots [0 .. hotbarSlotCount-1]
    private InventorySlot[] slots;
    public int TotalSlots => rows * columns;
    

    // ------------------------------------------------------------------
    // Events — the UI listens to these instead of polling
    // ------------------------------------------------------------------
    [HideInInspector] public UnityEvent<int> OnSlotChanged;      // slot index that changed
    [HideInInspector] public UnityEvent OnInventoryOpened;
    [HideInInspector] public UnityEvent OnInventoryClosed;

    // ------------------------------------------------------------------
    // State
    // ------------------------------------------------------------------
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        InitialiseSlots();
    }

    private void InitialiseSlots()
    {
        slots = new InventorySlot[TotalSlots];
        for (int i = 0; i < TotalSlots; i++)
            slots[i] = new InventorySlot();
    }

    // ------------------------------------------------------------------
    // Public API — Adding Items
    // ------------------------------------------------------------------

    /// <summary>
    /// Tries to add a quantity of an item. Returns the leftover amount that didn't fit.
    /// </summary>
    public int TryAddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return quantity;

        int remaining = quantity;

        // 1. Fill existing stacks first
        if (item.isStackable)
        {
            for (int i = 0; i < TotalSlots && remaining > 0; i++)
            {
                if (slots[i].item == item && slots[i].quantity < item.maxStackSize)
                {
                    int canFit = slots[i].RemainingSpace();
                    int toAdd = Mathf.Min(remaining, canFit);
                    slots[i].quantity += toAdd;
                    remaining -= toAdd;
                    OnSlotChanged?.Invoke(i);
                }
            }
        }

        // 2. Use empty slots for the remainder
        for (int i = 0; i < TotalSlots && remaining > 0; i++)
        {
            if (slots[i].IsEmpty)
            {
                int toAdd = item.isStackable ? Mathf.Min(remaining, item.maxStackSize) : 1;
                slots[i].item = item;
                slots[i].quantity = toAdd;
                remaining -= toAdd;
                OnSlotChanged?.Invoke(i);
            }
        }

        return remaining; // 0 means everything fit
    }

    /// <summary>Returns true if at least one of the item fits.</summary>
    public bool CanAddItem(ItemData item, int quantity = 1)
    {
        int space = 0;
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
                space += item.isStackable ? item.maxStackSize : 1;
            else if (slot.item == item && item.isStackable)
                space += slot.RemainingSpace();
            if (space >= quantity) return true;
        }
        return false;
    }

    // ------------------------------------------------------------------
    // Public API — Removing Items
    // ------------------------------------------------------------------

    /// <summary>
    /// Removes a quantity of an item from anywhere in the inventory.
    /// Returns true if the full amount was removed.
    /// </summary>
    public bool TryRemoveItem(ItemData item, int quantity = 1)
    {
        if (!HasItem(item, quantity)) return false;

        int remaining = quantity;
        for (int i = TotalSlots - 1; i >= 0 && remaining > 0; i--)
        {
            if (slots[i].item == item)
            {
                int toRemove = Mathf.Min(remaining, slots[i].quantity);
                slots[i].quantity -= toRemove;
                remaining -= toRemove;
                if (slots[i].quantity <= 0) slots[i].Clear();
                OnSlotChanged?.Invoke(i);
            }
        }
        return true;
    }

    /// <summary>Remove all items from a specific slot index.</summary>
    public void ClearSlot(int index)
    {
        if (!IsValidIndex(index)) return;
        slots[index].Clear();
        OnSlotChanged?.Invoke(index);
    }

    // ------------------------------------------------------------------
    // Public API — Querying
    // ------------------------------------------------------------------

    public InventorySlot GetSlot(int index)
    {
        return IsValidIndex(index) ? slots[index] : null;
    }

    public bool HasItem(ItemData item, int quantity = 1)
    {
        int count = 0;
        foreach (var slot in slots)
            if (slot.item == item) count += slot.quantity;
        return count >= quantity;
    }

    public int CountItem(ItemData item)
    {
        int count = 0;
        foreach (var slot in slots)
            if (slot.item == item) count += slot.quantity;
        return count;
    }

    // ------------------------------------------------------------------
    // Public API — Slot Manipulation
    // ------------------------------------------------------------------

    /// <summary>
    /// Swaps the contents of two slots. Used by drag-and-drop.
    /// If the items match and are stackable, merges them instead.
    /// </summary>
    public void SwapSlots(int fromIndex, int toIndex)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex)) return;
        if (fromIndex == toIndex) return;

        InventorySlot from = slots[fromIndex];
        InventorySlot to = slots[toIndex];

        // Merge stacks if same item
        if (!from.IsEmpty && !to.IsEmpty && from.item == to.item && from.item.isStackable)
        {
            int canFit = to.RemainingSpace();
            int toMove = Mathf.Min(from.quantity, canFit);
            to.quantity += toMove;
            from.quantity -= toMove;
            if (from.quantity <= 0) from.Clear();
        }
        else
        {
            // Full swap
            InventorySlot temp = new InventorySlot();
            temp.CopyFrom(from);
            from.CopyFrom(to);
            to.CopyFrom(temp);
        }

        OnSlotChanged?.Invoke(fromIndex);
        OnSlotChanged?.Invoke(toIndex);
    }

    /// <summary>Split a stack: move half (rounded down) to an empty target slot.</summary>
    public bool TrySplitStack(int fromIndex, int toIndex)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex)) return false;

        InventorySlot from = slots[fromIndex];
        InventorySlot to = slots[toIndex];

        if (from.IsEmpty || !from.item.isStackable || from.quantity < 2) return false;
        if (!to.IsEmpty) return false;

        int half = from.quantity / 2;
        to.item = from.item;
        to.quantity = half;
        from.quantity -= half;

        OnSlotChanged?.Invoke(fromIndex);
        OnSlotChanged?.Invoke(toIndex);
        return true;
    }

    // ------------------------------------------------------------------
    // Inventory Open / Close
    // ------------------------------------------------------------------

    public void OpenInventory()
    {
        if (IsOpen) return;
        IsOpen = true;
        OnInventoryOpened?.Invoke();
    }

    public void CloseInventory()
    {
        if (!IsOpen) return;
        IsOpen = false;
        OnInventoryClosed?.Invoke();
    }

    public void ToggleInventory()
    {
        if (IsOpen) CloseInventory();
        else OpenInventory();
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private bool IsValidIndex(int index) => index >= 0 && index < TotalSlots;

    // Useful for saving — returns a copy of all slot data
    public InventorySlot[] GetAllSlots() => slots;
}