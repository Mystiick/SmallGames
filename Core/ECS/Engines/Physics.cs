using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

using MystiickCore.ECS.Components;

namespace MystiickCore.ECS.Engines;

public static class Physics
{
    /// <summary>
    /// Gets the intersection distance between <paramref name="r"/> and <paramref name="targetCollider"/>
    /// </summary>
    public static float? CastRay(Ray r, BoxCollider targetCollider)
    {
        BoundingBox bb = new BoundingBox(
            new Vector3(targetCollider.WorldBoundingBox.Location.ToVector2(), 0f),
            new Vector3((targetCollider.WorldBoundingBox.Location + targetCollider.WorldBoundingBox.Size).ToVector2(), 0f)
        );

        return r.Intersects(bb);
    }

    /// <summary>
    /// Casts a Ray from the <paramref name="origin"/> toward <paramref name="direction"/>. 
    /// If a collision with <paramref name="targetCollider"/> is found at any range, the output float? will have a value
    /// </summary>
    /// <param name="origin">World location to begin the ray</param>
    /// <param name="direction">Direction *relative to the origin* to shoot the ray</param>
    /// <param name="targetCollider">Collider to check</param>
    public static float? CastRay(Vector2 origin, Vector2 direction, BoxCollider targetCollider)
    {
        Ray r = new(new Vector3(origin, 0f), new Vector3(direction, 0f));
        return CastRay(r, targetCollider);
    }

    /// <summary>
    /// Gets all colliding entities between <paramref name="origin"/> and <paramref name="direction"/>
    /// </summary>
    public static Entity[] CastAllToward(Vector2 origin, Vector2 destination, List<Entity> targets)
    {
        Vector2 direction = destination - origin;
        float distance = Vector2.Distance(origin, destination);

        // Need to normalize it after calculating the distance, otherwise the distance would always be between 0 and 1
        direction.Normalize();

        return Physics.CastAll(origin, direction, distance, targets);
    }


    /// <summary>
    /// Gets all colliding entities between <paramref name="origin"/> and <paramref name="direction"/>*<paramref name="maxDistance"/>
    /// </summary>
    public static Entity[] CastAll(Vector2 origin, Vector2 direction, float maxDistance, List<Entity> targets)
    {
        List<Entity> output = new();
        Ray r = new Ray(new Vector3(origin, 0f), new Vector3(direction, 0f));

        for (int i = 0; i < targets.Count; i++)
        {
            Entity temp = targets[i];
            float? rayDistance;

            if (temp.Collider != null)
            {
                rayDistance = CastRay(r, temp.Collider);

                if (rayDistance.HasValue && rayDistance.Value < maxDistance)
                {
                    output.Add(targets[i]);
                }
            }
        }

        return output.ToArray();
    }

    /// <summary>
    /// Resolves a BoxCollider collision between a moving entity and all entities is has collided with.
    /// First resolves the entity using the Target X with Source Y, and then the Target Y entity using
    /// </summary>
    public static Vector2 ResolveCollisions(Entity mover, List<Entity> collided)
    {
        foreach (Entity e in collided)
        {
            BoxCollider temp = mover.Collider.Copy();

            // Resolve target X
            temp.TargetBoundingBox = new Rectangle(
                temp.TargetBoundingBox.X,
                mover.Collider.WorldBoundingBox.Y,
                temp.WorldBoundingBox.Width,
                temp.WorldBoundingBox.Height
            );
            Point pX = ResolveCollision(temp, e.Collider);

            // Resolve target Y
            temp.TargetBoundingBox = new Rectangle(
                mover.Collider.WorldBoundingBox.X,
                mover.Collider.TargetBoundingBox.Y,
                temp.WorldBoundingBox.Width,
                temp.WorldBoundingBox.Height
            );
            Point pY = ResolveCollision(temp, e.Collider);

            mover.Collider.TargetBoundingBox = new Rectangle(pX.X, pY.Y, mover.Collider.TargetBoundingBox.Width, mover.Collider.TargetBoundingBox.Height);
        }

        return new Vector2(mover.Collider.TargetBoundingBox.X, mover.Collider.TargetBoundingBox.Y);
    }

    public static List<Entity> GetOverlappingEntities(Entity entity, List<Entity> allEntities)
    {
        List<Entity> output = new();

        for (int i = 0; i < allEntities.Count; i++)
        {
            Entity other = allEntities[i];

            // Don't need to check this with itself
            if (entity.ID == other.ID)
            {
                continue;
            }

            // Normal collision check
            if (entity.Collider.TargetBoundingBox.Intersects(other.Collider.WorldBoundingBox))
            {
                output.Add(other);
            }
            else if (entity.Collider.Continuous && entity.HasComponent<Velocity>())
            {
                // Distance between the last location and the current location of the current entity
                float distance = Vector2.Distance(entity.Collider.WorldBoundingBox.Center.ToVector2(), entity.Collider.TargetBoundingBox.Center.ToVector2());

                // Shoot a ray from the last location towards the current location
                var val = Physics.CastRay(entity.Collider.WorldBoundingBox.Center.ToVector2(), entity.GetComponent<Velocity>().Direction, other.Collider);

                // If there's a collision, and the collision is less than the distance between the old and new location, there was a raycast hit
                if (val.HasValue && val.Value <= distance)
                {
                    output.Add(other);
                }
            }
        }

        return output.ToList();
    }

    private static Point ResolveCollision(BoxCollider e1, BoxCollider e2)
    {
        if (e1.TargetBoundingBox.Intersects(e2.WorldBoundingBox))
        {
            // Determine directions traveling. 

            // Moving right = (0,0) position, but (1+,0) target
            // Moving down = (0,0) position but (0,1+)
            int xOffset = e1.WorldBoundingBox.X - e1.TargetBoundingBox.X;
            int yOffset = e1.WorldBoundingBox.Y - e1.TargetBoundingBox.Y;

            var totalOffset = new Point(
                xOffset == 0 ? 0 : -(xOffset / Math.Abs(xOffset)), // If moving left, X = -1
                yOffset == 0 ? 0 : -(yOffset / Math.Abs(yOffset))  // If moving up, X = -1
            );

            return new Point(
                EasyClamp(
                    ResolveDimension(e1.TargetBoundingBox.X, e1.TargetBoundingBox.Width, e2.TargetBoundingBox.X, e2.TargetBoundingBox.Width, totalOffset.X),
                    e1.WorldBoundingBox.X,
                    e1.TargetBoundingBox.X
                ),
                EasyClamp(
                    ResolveDimension(e1.TargetBoundingBox.Y, e1.TargetBoundingBox.Height, e2.TargetBoundingBox.Y, e2.TargetBoundingBox.Height, totalOffset.Y),
                    e1.WorldBoundingBox.Y,
                    e1.TargetBoundingBox.Y
                )
            );
        }
        else
        {
            return new Point(e1.TargetBoundingBox.X, e1.TargetBoundingBox.Y);
        }
    }

    private static int EasyClamp(int value, int i1, int i2)
    {
        if (i1 < i2)
            return Math.Clamp(value, i1, i2);
        else
            return Math.Clamp(value, i2, i1);
    }

    // TODO: Cleanup
    private static int ResolveDimension(int d1, int size1, int d2, int size2, int direction)
    {

        // Determine if X is overlapping
        // Is E1.X between E2.X and E2.X+Width
        // Or
        // Is E1.X+Width between E2.X and E2.X+Width
        if (d1 > d2 && d1 < d2 + size2)
        {
            // d1 Overlaps
        }
        else if (d1 + size1 > d2 && d1 + size1 < d2 + size2)
        {
            // d1+size1 overlaps
        }
        else
        {
            // Nothing overlaps, just kick it
            return d1;
        }

        // Determine which direction to punt the dimension
        if (direction < 0)
        {
            return d2 + size2;
        }
        else if (direction > 0)
        {
            return d2 - size1;
        }

        return d1;
    }
}
