﻿using Sanatana.EntityFrameworkCore.BatchSpecs.TestTools.Interfaces;
using NUnit.Framework;
using Sanatana.EntityFrameworkCore.Batch.Commands;
using SpecsFor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Should;
using SpecsFor.StructureMap;
using Sanatana.EntityFrameworkCore.Batch.Commands.Merge;
using Sanatana.EntityFrameworkCore.BatchSpecs.Samples.Entities;
using Sanatana.EntityFrameworkCore.BatchSpecs.Samples;

namespace Sanatana.EntityFrameworkCore.BatchSpecs.Specs
{
    public class RepositorySpecs
    {

        [TestFixture]
        public class when_inserting : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            public SampleDbContext SampleDatabase { get; set; }

            [Test]
            public void then_it_inserts()
            {
                var entity = new ParentEntity
                {
                    CreatedTime = DateTime.Now,
                    Embedded = new EmbeddedEntity
                    {
                        Address = "address1",
                        IsActive = true
                    }
                };


                SUT.Context.Set<ParentEntity>().Add(entity);
                SUT.Context.SaveChanges();
            }
        }


        [TestFixture]
        public class when_inserting_generic : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            public SampleDbContext SampleDatabase { get; set; }

            [Test]
            public void then_it_inserts_generic()
            {
                var entity = new GenericEntity<int>
                {
                    EntityId = 0,
                    Name = "Name1"
                };

                SUT.Context.Set<GenericEntity<int>>().Add(entity);
                SUT.Context.SaveChanges();
            }
        }


        [TestFixture]
        public class when_inserting_multiple_entities : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            public SampleDbContext SampleDatabase { get; set; }

            [Test]
            public void then_it_inserts_multiple_entities()
            {
                int insertCount = 15;
                var entities = new List<SampleEntity>();
                for (int i = 0; i < insertCount; i++)
                {
                    entities.Add(new SampleEntity
                    {
                        GuidNullableProperty = null,
                        DateProperty = DateTime.MinValue,
                        GuidProperty = Guid.NewGuid()
                    });
                }

                InsertCommand<SampleEntity> command = SUT.Insert<SampleEntity>();
                command.Insert.ExcludeProperty(x => x.Id);
                int changes = command.Execute(entities);

                changes.ShouldEqual(insertCount);
            }
        }

        [TestFixture]
        public class when_inserting_multiple_entities_with_output : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            public SampleDbContext SampleDatabase { get; set; }

            [Test]
            public void then_it_inserts_multiple_entities_and_outputs_ids()
            {
                int insertCount = 15;
                var entities = new List<SampleEntity>();
                for (int i = 0; i < insertCount; i++)
                {
                    entities.Add(new SampleEntity
                    {
                        GuidNullableProperty = null,
                        DateProperty = DateTime.UtcNow,
                        GuidProperty = Guid.NewGuid()
                    });
                }

                InsertCommand<SampleEntity> command = SUT.Insert<SampleEntity>();
                command.Insert.ExcludeProperty(x => x.Id);
                command.Output.IncludeProperty(x => x.Id);
                int changes = command.Execute(entities);

                changes.ShouldEqual(insertCount);
                entities.ForEach(
                    (entity) => entity.Id.ShouldNotEqual(0));
            }
        }

        [TestFixture]
        public class when_inserting_collection_navigation_property : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            private OneToManyEntity _parentEntity;
            public SampleDbContext SampleDatabase { get; set; }

            protected override void Given()
            {
                _parentEntity = new OneToManyEntity
                {
                    Name = "parent1"
                };
                SampleDatabase.Set<OneToManyEntity>().Add(_parentEntity);
                SampleDatabase.SaveChanges();
            }

            [Test]
            public void then_it_inserts_collection_navigation_property()
            {
                var entities = new List<ManyToOneEntity>
                {
                    new ManyToOneEntity
                    {
                        Name = "name1",
                        OneToManyEntityId = _parentEntity.OneToManyEntityId
                    },
                    new ManyToOneEntity
                    {
                        Name = "name2",
                        OneToManyEntityId = _parentEntity.OneToManyEntityId
                    }
                };

                InsertCommand<ManyToOneEntity> command = SUT.Insert<ManyToOneEntity>();
                command.Insert.ExcludeProperty(x => x.ManyToOneEntityId);
                int changes = command.Execute(entities);

                changes.ShouldEqual(entities.Count);
            }
        }

        [TestFixture]
        public class when_inserting_complex_property : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            public SampleDbContext SampleDatabase { get; set; }

            [Test]
            public void then_it_inserts_complex_property()
            {
                var entities = new List<ParentEntity>
                {
                    new ParentEntity
                    {
                        CreatedTime = DateTime.Now,
                        Embedded = new EmbeddedEntity
                        {
                            Address = "address1",
                            IsActive = true
                        }
                    },
                    new ParentEntity
                    {
                        CreatedTime = DateTime.Now.AddDays(1),
                        Embedded = new EmbeddedEntity
                        {
                            Address = "address2",
                            IsActive = true
                        }
                    }
                };

                InsertCommand<ParentEntity> command = SUT.Insert<ParentEntity>();
                command.Insert.ExcludeProperty(x => x.ParentEntityId);
                int changes = command.Execute(entities);

                changes.ShouldEqual(entities.Count);
            }
        }

        [TestFixture]
        public class when_deleting_many : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            private Guid _commonGuidValue = Guid.NewGuid();
            private int _entitiesCount = 10;
            public SampleDbContext SampleDatabase { get; set; }

            protected override void Given()
            {
                var entities = new List<SampleEntity>();
                for (int i = 0; i < _entitiesCount; i++)
                {
                    entities.Add(new SampleEntity
                    {
                        GuidNullableProperty = null,
                        DateProperty = DateTime.UtcNow,
                        GuidProperty = _commonGuidValue
                    });
                }

                InsertCommand<SampleEntity> command = SUT.Insert<SampleEntity>();
                command.Insert.ExcludeProperty(x => x.Id);
                int changes = command.Execute(entities);
            }

            [Test]
            public void then_it_deletes_multiple_entities()
            {
                int changes = SUT.DeleteMany<SampleEntity>(x => x.GuidProperty == _commonGuidValue);

                changes.ShouldEqual(_entitiesCount);
            }
        }

        [TestFixture]
        public class when_deleting_by_complex_property : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            private string _address = Guid.NewGuid().ToString();
            private int _entitiesCount = 10;
            public SampleDbContext SampleDatabase { get; set; }

            protected override void Given()
            {
                var entities = new List<ParentEntity>();
                for (int i = 0; i < _entitiesCount; i++)
                {
                    entities.Add(new ParentEntity
                    {
                        CreatedTime = DateTime.Now.AddMinutes(i),
                        Embedded = new EmbeddedEntity
                        {
                            Address = _address,
                            IsActive = true
                        }
                    });
                }

                InsertCommand<ParentEntity> command = SUT.Insert<ParentEntity>();
                command.Insert.ExcludeProperty(x => x.ParentEntityId);
                int changes = command.Execute(entities);
            }

            [Test]
            public void then_it_deletes_by_complex_property()
            {
                int changes = SUT.DeleteMany<ParentEntity>(x => x.Embedded.Address == _address);

                changes.ShouldEqual(_entitiesCount);
            }
        }

        [TestFixture]
        public class when_updating_many : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            private Guid _commonGuidValue = Guid.NewGuid();
            private int _entitiesCount = 10;
            public SampleDbContext SampleDatabase { get; set; }

            protected override void Given()
            {
                var entities = new List<SampleEntity>();
                for (int i = 0; i < _entitiesCount; i++)
                {
                    entities.Add(new SampleEntity
                    {
                        GuidNullableProperty = null,
                        DateProperty = DateTime.UtcNow,
                        GuidProperty = _commonGuidValue
                    });
                }

                InsertCommand<SampleEntity> command = SUT.Insert<SampleEntity>();
                command.Insert.ExcludeProperty(x => x.Id);
                int changes = command.Execute(entities);
            }

            [Test]
            public void then_it_updates_multiple_entities()
            {
                UpdateCommand<SampleEntity> updateOp = SUT.UpdateMany<SampleEntity>(
                    x => x.GuidProperty == _commonGuidValue);
                updateOp.Assign(x => x.DateProperty, x => DateTime.Now);
                int changes = updateOp.Execute();

                changes.ShouldEqual(_entitiesCount);
            }
        }

        [TestFixture]
        public class when_updating_limited_number_of_rows : SpecsFor<Repository>
           , INeedSampleDatabase
        {
            private Guid _commonGuidValue = Guid.NewGuid();
            private int _entitiesCount = 10;
            private int _limit = 2;
            private int _updateChanges;
            public SampleDbContext SampleDatabase { get; set; }

            protected override void Given()
            {
                var entities = new List<SampleEntity>();
                for (int i = 0; i < _entitiesCount; i++)
                {
                    entities.Add(new SampleEntity
                    {
                        GuidNullableProperty = null,
                        DateProperty = DateTime.UtcNow,
                        GuidProperty = _commonGuidValue
                    });
                }

                InsertCommand<SampleEntity> command = SUT.Insert<SampleEntity>();
                command.Insert.ExcludeProperty(x => x.Id);
                int changes = command.Execute(entities);


                //execute limited Update
                UpdateCommand<SampleEntity> updateOp = SUT
                    .UpdateMany<SampleEntity>(x => x.GuidProperty == _commonGuidValue)
                    .Assign(x => x.IntProperty, x => 5)
                    .SetLimit(_limit);
                _updateChanges = updateOp.Execute();
            }

            [Test]
            public void then_it_updates_expected_number_of_entities()
            {
                _updateChanges.ShouldEqual(_limit);
            }

            [Test]
            public void then_it_returns_updated_entities()
            {
                SampleEntity[] allEntities = SUT.Context.Set<SampleEntity>()
                    .Where(x => x.GuidProperty == _commonGuidValue)
                    .ToArray();

                allEntities.Count(x => x.IntProperty == 5).ShouldEqual(_limit);

                int expectedUnchangedCount = _entitiesCount - _limit;
                allEntities.Count(x => x.IntProperty != 5).ShouldEqual(expectedUnchangedCount);
            }
        }

        [TestFixture]
        public class when_selecting_page : SpecsFor<Repository>
         , INeedSampleDatabase
        {
            private Guid _commonGuidValue = Guid.NewGuid();
            private int _entitiesCount = 15;
            public SampleDbContext SampleDatabase { get; set; }

            protected override void Given()
            {
                var entities = new List<SampleEntity>();
                for (int i = 0; i < _entitiesCount; i++)
                {
                    entities.Add(new SampleEntity
                    {
                        GuidNullableProperty = null,
                        DateProperty = DateTime.UtcNow,
                        GuidProperty = _commonGuidValue
                    });
                }

                InsertCommand<SampleEntity> command = SUT.Insert<SampleEntity>();
                command.Insert.ExcludeProperty(x => x.Id);
                int changes = command.Execute(entities);
            }

            [Test]
            public void then_it_selects_page_of_entities()
            {
                int pageSize = 10;

                RepositoryResult<SampleEntity> select = SUT.FindPage<SampleEntity, DateTime?>(
                    0, pageSize, true
                    , x => x.GuidProperty == _commonGuidValue
                    , x => x.DateProperty
                    , true);

                select.Data.Count.ShouldEqual(pageSize);
                select.TotalRows.ShouldEqual(_entitiesCount);
            }
        }


    }
}
