using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models;

namespace Elzik.Mecon.Framework.Tests.Unit.Infrastructure.TestData
{
    public static class TestMediaContainers
    {
        public static MediaContainer GetVideoMediaContainer()
        {
            return new MediaContainer
            {
                AllowSync = "1",
                Art = "/:/resources/movie-fanart.jpg",
                Identifier = "com.plexapp.plugins.library",
                LibrarySectionID = "1",
                LibrarySectionTitle = "Films",
                LibrarySectionUUID = "4915aff2-7a9a-4409-b275-29bad7a4cb94",
                MediaTagPrefix = "/system/bundle/media/flags/",
                MediaTagVersion = "1627505128",
                Size = "3",
                Thumb = "/:/resources/movie.png",
                Title1 = "Films",
                Title2 = "All Films",
                Video =
                    new System.Collections.Generic.List<
                        Video>
                    {
                        new Video
                        {
                            AddedAt = "1620835253",
                            Art = "/library/metadata/64143/art/1628558849",
                            ChapterSource = "media",
                            ContentRating = null,
                            Country =
                                new System.Collections.Generic.List<
                                    Country>
                                {
                                    new Country
                                    {
                                        Tag = "France"
                                    }
                                },
                            Director =
                                new System.Collections.Generic.List<Director>
                                {
                                    new Director
                                    {
                                        Tag = "Éric Rohmer"
                                    }
                                },
                            Duration = "5943124",
                            Genre =
                                new System.Collections.Generic.List<
                                    Genre>
                                {
                                    new Genre
                                    {
                                        Tag = "Comédie"
                                    },
                                    new Genre
                                    {
                                        Tag = "Drame"
                                    }
                                },
                            Guid = "com.plexapp.agents.imdb://tt0090565?lang=fr",
                            Key = "/library/metadata/64143",
                            LastRatedAt = null,
                            LastViewedAt = null,
                            Media =
                                new System.Collections.Generic.List<
                                    Media>
                                {
                                    new Media
                                    {
                                        AspectRatio = "1.33",
                                        AudioChannels = "1",
                                        AudioCodec = "flac",
                                        AudioProfile = null,
                                        Bitrate = "20099",
                                        Container = "mkv",
                                        Duration = "5943124",
                                        Has64bitOffsets = null,
                                        Height = "1080",
                                        Id = "134659",
                                        OptimizedForStreaming = null,
                                        Parts =
                                            new System.Collections.Generic.List<Part>
                                            {
                                                new Part
                                                {
                                                    AudioProfile = null,
                                                    Container = "mkv",
                                                    Duration = "5943124",
                                                    File =
                                                        "/Films/4 aventures de Reinette et Mirabelle 1987 1080p BluRay FLAC x264-EA.mkv",
                                                    Has64bitOffsets = null,
                                                    HasThumbnail = null,
                                                    Id = "147394",
                                                    Indexes = "sd",
                                                    Key = "/library/parts/147394/1423033728/file.mkv",
                                                    OptimizedForStreaming = null,
                                                    PacketLength = null,
                                                    Size = "14920095015",
                                                    VideoProfile = "high"
                                                }
                                            },
                                        VideoCodec = "h264",
                                        VideoFrameRate = "24p",
                                        VideoProfile = "high",
                                        VideoResolution = "1080",
                                        Width = "1480"
                                    }
                                },
                            OriginalTitle = null,
                            OriginallyAvailableAt = "1987-02-04",
                            PrimaryExtraKey = null,
                            Rating = "7.6",
                            RatingKey = "64143",
                            Role =
                                new System.Collections.Generic.List<
                                    Role>
                                {
                                    new Role
                                    {
                                        Tag = "Joëlle Miquel"
                                    },
                                    new Role
                                    {
                                        Tag = "Jessica Forde"
                                    },
                                    new Role
                                    {
                                        Tag = "Philippe Laudenbach"
                                    }
                                },
                            SkipCount = null,
                            Studio = "Compagnie Eric Rohmer",
                            Summary =
                                "Reinette, la souris des champs et Mirabelle, la souris des villes dans quatre aventures. À la campagne. À la recherche de l'heure bleue, cet instant magique que l'on peut saisir à l'aurore. Puis à Paris, où Reinette a rejoint Mirabelle pour suivre des cours de peinture. Ses tribulations parisiennes la mettent aux prises avec un garçon de café et un marchand de tableaux particulièrement bavards, puis avec un mendiant et une arnaqueuse...",
                            Tagline = null,
                            Thumb = "/library/metadata/64143/thumb/1628558849",
                            Title = "4 aventures de Reinette et Mirabelle",
                            TitleSort = null,
                            Type = "movie",
                            UpdatedAt = "1628558849",
                            UserRating = null,
                            ViewCount = null,
                            ViewOffset = null,
                            Writer =
                                new System.Collections.Generic.List<
                                    Writer>
                                {
                                    new Writer
                                    {
                                        Tag = "Éric Rohmer"
                                    }
                                },
                            Year = "1987"
                        },
                        new Video
                        {
                            AddedAt = "1620835308",
                            Art = "/library/metadata/64420/art/1623185035",
                            ChapterSource = "media",
                            ContentRating = "R",
                            Country =
                                new System.Collections.Generic.List<
                                    Country>
                                {
                                    new Country
                                    {
                                        Tag = "USA"
                                    }
                                },
                            Director =
                                new System.Collections.Generic.List<Director>
                                {
                                    new Director
                                    {
                                        Tag = "Mike Mills"
                                    }
                                },
                            Duration = "7124192",
                            Genre =
                                new System.Collections.Generic.List<
                                    Genre>
                                {
                                    new Genre
                                    {
                                        Tag = "Drama"
                                    }
                                },
                            Guid = "com.plexapp.agents.imdb://tt4385888?lang=en",
                            Key = "/library/metadata/64420",
                            LastRatedAt = null,
                            LastViewedAt = null,
                            Media =
                                new System.Collections.Generic.List<
                                    Media>
                                {
                                    new Media
                                    {
                                        AspectRatio = "1.85",
                                        AudioChannels = "6",
                                        AudioCodec = "dca",
                                        AudioProfile = "dts",
                                        Bitrate = "17768",
                                        Container = "mkv",
                                        Duration = "7124192",
                                        Has64bitOffsets = null,
                                        Height = "964",
                                        Id = "134938",
                                        OptimizedForStreaming = null,
                                        Parts =
                                            new System.Collections.Generic.List<Part>
                                            {
                                                new Part
                                                {
                                                    AudioProfile = "dts",
                                                    Container = "mkv",
                                                    Duration = "7124192",
                                                    File =
                                                        "/Films/20th.Century.Women.2016.1080p.BluRay.DTS.x264-VietHD/20th.Century.Women.2016.1080p.BluRay.DTS.x264-VietHD.mkv",
                                                    Has64bitOffsets = null,
                                                    HasThumbnail = null,
                                                    Id = "147749",
                                                    Indexes = "sd",
                                                    Key = "/library/parts/147749/1491136746/file.mkv",
                                                    OptimizedForStreaming = null,
                                                    PacketLength = null,
                                                    Size = "15825072616",
                                                    VideoProfile = "high"
                                                }
                                            },
                                        VideoCodec = "h264",
                                        VideoFrameRate = "24p",
                                        VideoProfile = "high",
                                        VideoResolution = "1080",
                                        Width = "1920"
                                    }
                                },
                            OriginalTitle = null,
                            OriginallyAvailableAt = "2016-10-08",
                            PrimaryExtraKey = "/library/metadata/66442",
                            Rating = "7.4",
                            RatingKey = "64420",
                            Role =
                                new System.Collections.Generic.List<
                                    Role>
                                {
                                    new Role
                                    {
                                        Tag = "Annette Bening"
                                    },
                                    new Role
                                    {
                                        Tag = "Elle Fanning"
                                    },
                                    new Role
                                    {
                                        Tag = "Greta Gerwig"
                                    }
                                },
                            SkipCount = null,
                            Studio = "Annapurna Pictures",
                            Summary =
                                "In 1979 Santa Barbara, California, Dorothea Fields is a determined single mother in her mid-50s who is raising her adolescent son, Jamie, at a moment brimming with cultural change and rebellion. Dorothea enlists the help of two younger women – Abbie, a free-spirited punk artist living as a boarder in the Fields' home and Julie, a savvy and provocative teenage neighbour – to help with Jamie's upbringing.",
                            Tagline = null,
                            Thumb = "/library/metadata/64420/thumb/1623185035",
                            Title = "20th Century Women",
                            TitleSort = null,
                            Type = "movie",
                            UpdatedAt = "1623185035",
                            UserRating = null,
                            ViewCount = null,
                            ViewOffset = null,
                            Writer =
                                new System.Collections.Generic.List<
                                    Writer>
                                {
                                    new Writer
                                    {
                                        Tag = "Mike Mills"
                                    }
                                },
                            Year = "2016"
                        },
                        new Video
                        {
                            AddedAt = "1620835253",
                            Art = "/library/metadata/64142/art/1628558849",
                            ChapterSource = "media",
                            ContentRating = "R",
                            Country =
                                new System.Collections.Generic.List<
                                    Country>
                                {
                                    new Country
                                    {
                                        Tag = "Spain"
                                    },
                                    new Country
                                    {
                                        Tag = "United Kingdom"
                                    }
                                },
                            Director =
                                new System.Collections.Generic.List<Director>
                                {
                                    new Director
                                    {
                                        Tag = "Juan Carlos Fresnadillo"
                                    }
                                },
                            Duration = "6006048",
                            Genre =
                                new System.Collections.Generic.List<
                                    Genre>
                                {
                                    new Genre
                                    {
                                        Tag = "Horror"
                                    },
                                    new Genre
                                    {
                                        Tag = "Thriller"
                                    }
                                },
                            Guid = "com.plexapp.agents.imdb://tt0463854?lang=en",
                            Key = "/library/metadata/64142",
                            LastRatedAt = null,
                            LastViewedAt = null,
                            Media =
                                new System.Collections.Generic.List<
                                    Media>
                                {
                                    new Media
                                    {
                                        AspectRatio = "1.85",
                                        AudioChannels = "6",
                                        AudioCodec = "dca",
                                        AudioProfile = "dts",
                                        Bitrate = "22304",
                                        Container = "mkv",
                                        Duration = "6006048",
                                        Has64bitOffsets = null,
                                        Height = "1040",
                                        Id = "134658",
                                        OptimizedForStreaming = null,
                                        Parts =
                                            new System.Collections.Generic.List<Part>
                                            {
                                                new Part
                                                {
                                                    AudioProfile = "dts",
                                                    Container = "mkv",
                                                    Duration = "6006048",
                                                    File = "/Films/28.Weeks.Later.2007.1080p.BluRay.DTS.x264-IDE.mkv",
                                                    Has64bitOffsets = null,
                                                    HasThumbnail = null,
                                                    Id = "147393",
                                                    Indexes = "sd",
                                                    Key = "/library/parts/147393/1425049087/file.mkv",
                                                    OptimizedForStreaming = null,
                                                    PacketLength = null,
                                                    Size = "16712596293",
                                                    VideoProfile = "high"
                                                }
                                            },
                                        VideoCodec = "h264",
                                        VideoFrameRate = "24p",
                                        VideoProfile = "high",
                                        VideoResolution = "1080",
                                        Width = "1912"
                                    }
                                },
                            OriginalTitle = null,
                            OriginallyAvailableAt = "2007-05-11",
                            PrimaryExtraKey = "/library/metadata/64418",
                            Rating = "6.6",
                            RatingKey = "64142",
                            Role =
                                new System.Collections.Generic.List<
                                    Role>
                                {
                                    new Role
                                    {
                                        Tag = "Robert Carlyle"
                                    },
                                    new Role
                                    {
                                        Tag = "Rose Byrne"
                                    },
                                    new Role
                                    {
                                        Tag = "Jeremy Renner"
                                    }
                                },
                            SkipCount = null,
                            Studio = "DNA Films",
                            Summary =
                                "The inhabitants of the British Isles have lost their battle against the onslaught of disease, as the deadly rage virus has killed every citizen there. Six months later, a group of Americans dare to set foot on the isles, convinced the danger has come and gone. But it soon becomes all too clear that the scourge continues to live, waiting to pounce on its next victims.",
                            Tagline = "When days turn to weeks... the horror returns.",
                            Thumb = "/library/metadata/64142/thumb/1628558849",
                            Title = "28 Weeks Later",
                            TitleSort = null,
                            Type = "movie",
                            UpdatedAt = "1628558849",
                            UserRating = null,
                            ViewCount = null,
                            ViewOffset = null,
                            Writer =
                                new System.Collections.Generic.List<
                                    Writer>
                                {
                                    new Writer
                                    {
                                        Tag = "Juan Carlos Fresnadillo"
                                    },
                                    new Writer
                                    {
                                        Tag = "Enrique López Lavigne"
                                    }
                                },
                            Year = "2007"
                        }
                    },
                ViewGroup = "movie",
                ViewMode = "65592"
            };
        }
    }
}