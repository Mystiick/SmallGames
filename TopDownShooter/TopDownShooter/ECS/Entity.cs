using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using TopDownShooter.ECS.Components;
using TopDownShooter.Exceptions;

namespace TopDownShooter.ECS
{
    public class Entity
    {
        public Transform Transform { get; private set; }
        public BoxCollider Collider { get; private set; }
        public Sprite Sprite { get; private set; }
        public Guid ID { get; private set; }
        public bool Expired { get; set; }
        public string Name { get; set; }
        public int ComponentCount { get { return _components.Count; } }

        private List<Component> _components;

        public Entity()
        {
            _components = new List<Component>();
            this.ID = Guid.NewGuid();
        }

        public Entity(params Component[] components) : this() => this.AddComponents(components);
        public Entity(IEnumerable<Component> components) : this() => this.AddComponents(components);

        public void AddComponents(IEnumerable<Component> items)
        {
            foreach (Component item in items)
            {
                this.AddComponent(item);
            }
        }

        public void AddComponent(Component item)
        {
            // Make sure we aren't adding a duplicate to the list. Duplicates would berak the GetComponent<T> logic since we only expect 1 of each type
            // If you need duplicate components on an entity, you should use a component that has a List<DataTypeYouNeedDuplicated>
            // If that happens a lot, you should make a good way to get child subcomponents
            if (_components.Any(x => x.GetType() == item.GetType()))
            {
                throw new DuplicateComponentException($"Cannot add a duplicate component type of {item.GetType().FullName}");
            }

            // Check for common entity types, and assign to the public property if needed
            if (item.GetType() == typeof(Transform))
            {
                this.Transform = (Transform)item;
            }
            if (item.GetType() == typeof(BoxCollider))
            {
                this.Collider = (BoxCollider)item;
            }
            if (item.GetType() == typeof(Sprite))
            {
                this.Sprite = (Sprite)item;
            }

            // Add to the component bag
            _components.Add(item);
        }

        public void RemoveComponent<T>() where T : Component
        {
            _components.Remove(this.GetComponent<T>());
        }

        public T GetComponent<T>() where T : Component
        {
            return (T)_components.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        public bool HasComponent(Type t)
        {
            return _components.Any(x => x.GetType() == t);
        }

        public bool HasComponent<T>() where T : Component
        {
            return this.HasComponent(typeof(T));
        }
    }
}
