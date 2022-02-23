using System.Collections.Generic;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps.SymbolResolver
{
    public class SymbolResolver_NestedRoomMaze : RimWorld.BaseGen.SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            if (!rp.TryGetCustom("minRoomDimension", out int num))
            {
                Log.Error("Could not find a field minRoomDimension");
            }
            else
            {
                var width = rp.rect.Width;
                var height = rp.rect.Height;
                Log.Message($"Current nested room dimensions -> ({width}:{height})");
                if (width < 2 * num && height < 2 * num)
                {
                    MakeRoom(rp);
                }
                else if ((width < 4 * num || height < 4 * num) && Rand.Value < 0.2f)
                {
                    var rp2 = rp;
                    var rp3 = rp;
                    rp2.rect.Width = rp.rect.Width / 2;
                    rp3.rect.minX = rp2.rect.maxX;
                    rp3.rect.Width = rp.rect.Width / 2;
                    MakeRoom(rp2);
                    MakeRoom(rp3);
                }
                else if (width < 4 * num && height < 4 * num && Rand.Value < 0.1f)
                {
                    MakeRoom(rp);
                }
                else
                {
                    var list = new List<CellRect>();
                    var list2 = new List<CellRect>();
                    if (width > 2 * num)
                    {
                        list.Add(new CellRect(rp.rect.minX, rp.rect.minZ, rp.rect.Width / 2, rp.rect.Height));
                        list.Add(new CellRect(rp.rect.minX + (rp.rect.Width / 2) - 1, rp.rect.minZ, rp.rect.Width / 2,
                            rp.rect.Height));
                    }
                    else
                    {
                        list.Add(new CellRect(rp.rect.minX, rp.rect.minZ, rp.rect.Width, rp.rect.Height));
                    }

                    foreach (var item in list)
                    {
                        if (height > 2 * num)
                        {
                            list2.Add(new CellRect(item.minX, item.minZ, item.Width, item.Height / 2));
                            list2.Add(new CellRect(item.minX, item.minZ + (item.Height / 2) - 1, item.Width,
                                item.Height / 2));
                        }
                        else
                        {
                            list2.Add(item);
                        }
                    }

                    foreach (var rect in list2)
                    {
                        var resolveParams = rp;
                        resolveParams.rect = rect;
                        resolveParams.SetCustom("minRoomDimension", num);
                        BaseGen.symbolStack.Push("nestedRoomMaze", resolveParams);
                    }
                }
            }
        }

        private void MakeRoom(ResolveParams rp)
        {
            var array = new char[3];
            char[] source =
            {
                'N',
                'S',
                'E',
                'W'
            };
            array[0] = source.RandomElement();
            array[1] = source.RandomElement();
            if (array[1] == array[0])
            {
                array[1] = 'X';
            }

            rp.SetCustom("hasDoor", array);
            BaseGen.symbolStack.Push("roomWithDoor", rp);
        }
    }
}