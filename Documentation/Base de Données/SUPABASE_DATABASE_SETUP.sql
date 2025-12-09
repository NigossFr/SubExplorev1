-- =====================================================
-- SUBEXPLORE DATABASE SETUP SCRIPT FOR SUPABASE
-- =====================================================
-- Version: 1.0
-- Description: Script complet pour initialiser la base de données
-- IMPORTANT: Exécuter dans l'ordre dans Supabase SQL Editor
-- =====================================================

-- =====================================================
-- PARTIE 1: EXTENSIONS ET CONFIGURATION
-- =====================================================

-- Extensions nécessaires
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";      -- Pour les UUIDs
CREATE EXTENSION IF NOT EXISTS "postgis";        -- Pour les données géospatiales
CREATE EXTENSION IF NOT EXISTS "pg_trgm";        -- Pour la recherche fuzzy
CREATE EXTENSION IF NOT EXISTS "unaccent";       -- Pour la recherche sans accents
CREATE EXTENSION IF NOT EXISTS "pgcrypto";       -- Pour le chiffrement

-- Configuration de la recherche textuelle en français
CREATE TEXT SEARCH CONFIGURATION french_unaccent( COPY = french );
ALTER TEXT SEARCH CONFIGURATION french_unaccent
    ALTER MAPPING FOR hword, hword_part, word WITH unaccent, french_stem;

-- =====================================================
-- PARTIE 2: TYPES ET ENUMS
-- =====================================================

-- Types de compte
CREATE TYPE account_type AS ENUM (
    'Standard',
    'ExpertModerator',
    'VerifiedProfessional',
    'Administrator'
);

-- Statut d'abonnement
CREATE TYPE subscription_status AS ENUM (
    'Free',
    'Premium',
    'Professional',
    'Enterprise'
);

-- Niveau d'expertise
CREATE TYPE expertise_level AS ENUM (
    'Beginner',
    'Intermediate',
    'Advanced',
    'Expert',
    'Instructor'
);

-- Statut de modérateur
CREATE TYPE moderator_status AS ENUM (
    'None',
    'Probation',
    'Active',
    'Suspended'
);

-- Spécialisation de modérateur
CREATE TYPE moderator_specialization AS ENUM (
    'None',
    'RecreationalDiving',
    'TechnicalDiving',
    'Freediving',
    'Snorkeling',
    'Safety',
    'Marine Life',
    'Equipment'
);

-- Statut de validation des spots
CREATE TYPE spot_validation_status AS ENUM (
    'Draft',
    'Pending',
    'UnderReview',
    'RevisionRequested',
    'Approved',
    'Rejected'
);

-- Niveau de difficulté
CREATE TYPE difficulty_level AS ENUM (
    'Beginner',
    'Intermediate',
    'Advanced',
    'Expert',
    'TechnicalOnly'
);

-- Type d'eau
CREATE TYPE water_type AS ENUM (
    'Sea',
    'Lake',
    'River',
    'Quarry',
    'Pool',
    'Cave'
);

-- Force du courant
CREATE TYPE current_strength AS ENUM (
    'None',
    'Weak',
    'Moderate',
    'Strong',
    'VeryStrong'
);

-- Type d'accès
CREATE TYPE access_type AS ENUM (
    'Shore',
    'Boat',
    'Both'
);

-- Difficulté d'accès
CREATE TYPE access_difficulty AS ENUM (
    'Easy',
    'Moderate',
    'Difficult',
    'VeryDifficult'
);

-- Statut de vérification
CREATE TYPE verification_status AS ENUM (
    'Pending',
    'InProgress',
    'Verified',
    'Rejected',
    'Expired'
);

-- Statut de réservation
CREATE TYPE booking_status AS ENUM (
    'Pending',
    'Confirmed',
    'Cancelled',
    'Completed',
    'NoShow'
);

-- Statut de paiement
CREATE TYPE payment_status AS ENUM (
    'Pending',
    'Paid',
    'Failed',
    'Refunded',
    'PartiallyRefunded'
);

-- Type de conversation
CREATE TYPE conversation_type AS ENUM (
    'Direct',
    'Group',
    'Support',
    'System'
);

-- Type de message
CREATE TYPE message_type AS ENUM (
    'Text',
    'System',
    'SafetyReminder',
    'Notification'
);

-- Statut de modération
CREATE TYPE moderation_status AS ENUM (
    'Pending',
    'Approved',
    'Rejected',
    'Flagged'
);

-- =====================================================
-- PARTIE 3: TABLES PRINCIPALES
-- =====================================================

-- Table Users (profils utilisateurs étendus)
CREATE TABLE IF NOT EXISTS public.users (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    auth_id UUID UNIQUE NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
    
    -- Informations personnelles
    email VARCHAR(255) UNIQUE NOT NULL,
    username VARCHAR(30) UNIQUE,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    avatar_url TEXT,
    bio TEXT,
    birth_date DATE,
    phone VARCHAR(20),
    
    -- Localisation
    country VARCHAR(2),
    city VARCHAR(100),
    postal_code VARCHAR(20),
    default_location GEOGRAPHY(POINT, 4326),
    
    -- Type de compte et abonnement
    account_type account_type NOT NULL DEFAULT 'Standard',
    subscription_status subscription_status DEFAULT 'Free',
    subscription_end_date TIMESTAMP WITH TIME ZONE,
    
    -- Profil plongeur
    expertise_level expertise_level DEFAULT 'Beginner',
    years_of_practice INTEGER DEFAULT 0,
    favorite_activity VARCHAR(50),
    certifications JSONB DEFAULT '[]'::JSONB,
    equipment JSONB DEFAULT '[]'::JSONB,
    
    -- Préférences
    preferences JSONB DEFAULT '{
        "notifications": {
            "new_spots": true,
            "messages": true,
            "bookings": true,
            "community": true,
            "marketing": false
        },
        "privacy": {
            "profile_visible": true,
            "show_in_buddy_finder": false,
            "accept_messages": true,
            "share_email": false,
            "share_phone": false
        },
        "display": {
            "language": "fr",
            "units": "metric",
            "theme": "light",
            "map_type": "standard"
        }
    }'::JSONB,
    
    -- Modération
    moderator_specialization moderator_specialization DEFAULT 'None',
    moderator_status moderator_status DEFAULT 'None',
    moderator_since TIMESTAMP WITH TIME ZONE,
    permissions INTEGER DEFAULT 1,
    
    -- Statistiques
    stats JSONB DEFAULT '{
        "spots_created": 0,
        "spots_validated": 0,
        "reviews_written": 0,
        "photos_uploaded": 0,
        "dives_logged": 0,
        "buddies_met": 0,
        "bookings_made": 0,
        "articles_written": 0
    }'::JSONB,
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    last_login TIMESTAMP WITH TIME ZONE,
    is_active BOOLEAN DEFAULT true,
    is_email_verified BOOLEAN DEFAULT false,
    deleted_at TIMESTAMP WITH TIME ZONE,
    
    -- Contraintes
    CONSTRAINT chk_email CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'),
    CONSTRAINT chk_username CHECK (username IS NULL OR username ~* '^[a-z0-9_]{3,30}$'),
    CONSTRAINT chk_age CHECK (birth_date IS NULL OR birth_date <= CURRENT_DATE - INTERVAL '13 years'),
    CONSTRAINT chk_phone CHECK (phone IS NULL OR phone ~* '^\+?[0-9]{8,20}$')
);

-- Table Spots (sites de plongée)
CREATE TABLE IF NOT EXISTS public.spots (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    creator_id UUID NOT NULL REFERENCES public.users(id),
    
    -- Informations de base
    name VARCHAR(200) NOT NULL,
    slug VARCHAR(250) UNIQUE NOT NULL,
    description TEXT NOT NULL,
    
    -- Localisation
    location GEOGRAPHY(POINT, 4326) NOT NULL,
    address TEXT,
    city VARCHAR(100),
    region VARCHAR(100),
    country VARCHAR(2) NOT NULL,
    postal_code VARCHAR(20),
    
    -- Caractéristiques
    spot_type VARCHAR(50) NOT NULL,
    secondary_types VARCHAR(50)[] DEFAULT '{}',
    activities VARCHAR(50)[] NOT NULL,
    difficulty_level difficulty_level NOT NULL,
    
    -- Conditions
    water_type water_type NOT NULL,
    max_depth DECIMAL(5,2),
    average_depth DECIMAL(5,2),
    visibility_min DECIMAL(4,1),
    visibility_max DECIMAL(4,1),
    current_strength current_strength DEFAULT 'None',
    water_temperature_summer DECIMAL(3,1),
    water_temperature_winter DECIMAL(3,1),
    
    -- Accès
    access_type access_type DEFAULT 'Shore',
    access_difficulty access_difficulty DEFAULT 'Easy',
    facilities JSONB DEFAULT '[]'::JSONB,
    required_equipment JSONB DEFAULT '[]'::JSONB,
    rental_equipment_available BOOLEAN DEFAULT false,
    
    -- Sécurité
    safety_notes TEXT,
    emergency_contacts JSONB DEFAULT '[]'::JSONB,
    nearest_decompression_chamber JSONB,
    hazards JSONB DEFAULT '[]'::JSONB,
    
    -- Biodiversité
    marine_life JSONB DEFAULT '[]'::JSONB,
    points_of_interest JSONB DEFAULT '[]'::JSONB,
    best_months INTEGER[] DEFAULT '{}',
    
    -- Média
    cover_image_url TEXT,
    images JSONB DEFAULT '[]'::JSONB,
    
    -- Validation
    validation_status spot_validation_status DEFAULT 'Pending',
    validated_by UUID REFERENCES public.users(id),
    validated_at TIMESTAMP WITH TIME ZONE,
    validation_notes TEXT,
    revision_requested BOOLEAN DEFAULT false,
    revision_reason TEXT,
    
    -- Popularité
    average_rating DECIMAL(2,1) DEFAULT 0.0,
    total_ratings INTEGER DEFAULT 0,
    total_reviews INTEGER DEFAULT 0,
    total_favorites INTEGER DEFAULT 0,
    popularity_score DECIMAL(5,2) DEFAULT 0.0,
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    published_at TIMESTAMP WITH TIME ZONE,
    last_reviewed_at TIMESTAMP WITH TIME ZONE,
    is_active BOOLEAN DEFAULT true,
    deleted_at TIMESTAMP WITH TIME ZONE,
    
    -- SEO
    meta_title VARCHAR(160),
    meta_description VARCHAR(320),
    tags VARCHAR(50)[] DEFAULT '{}',
    
    -- Contraintes
    CONSTRAINT chk_depth CHECK (
        (max_depth IS NULL OR max_depth >= 0 AND max_depth <= 500) AND
        (average_depth IS NULL OR average_depth >= 0 AND average_depth <= max_depth)
    ),
    CONSTRAINT chk_visibility CHECK (
        (visibility_min IS NULL OR visibility_min >= 0 AND visibility_min <= 100) AND
        (visibility_max IS NULL OR visibility_max >= 0 AND visibility_max <= 100) AND
        (visibility_max IS NULL OR visibility_min IS NULL OR visibility_max >= visibility_min)
    ),
    CONSTRAINT chk_temperature CHECK (
        (water_temperature_summer IS NULL OR water_temperature_summer >= -2 AND water_temperature_summer <= 40) AND
        (water_temperature_winter IS NULL OR water_temperature_winter >= -2 AND water_temperature_winter <= 40)
    ),
    CONSTRAINT chk_rating CHECK (average_rating >= 0 AND average_rating <= 5)
);

-- Table Structures (clubs et centres)
CREATE TABLE IF NOT EXISTS public.structures (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    owner_id UUID NOT NULL REFERENCES public.users(id),
    
    -- Informations de base
    name VARCHAR(200) NOT NULL,
    slug VARCHAR(250) UNIQUE NOT NULL,
    legal_name VARCHAR(200),
    registration_number VARCHAR(100),
    structure_type VARCHAR(50) NOT NULL,
    
    -- Description
    description TEXT NOT NULL,
    short_description VARCHAR(500),
    
    -- Localisation
    location GEOGRAPHY(POINT, 4326) NOT NULL,
    address TEXT NOT NULL,
    city VARCHAR(100) NOT NULL,
    region VARCHAR(100),
    country VARCHAR(2) NOT NULL,
    postal_code VARCHAR(20),
    
    -- Contact
    phone VARCHAR(20),
    mobile VARCHAR(20),
    email VARCHAR(255),
    website TEXT,
    social_media JSONB DEFAULT '{}'::JSONB,
    
    -- Services
    services JSONB DEFAULT '[]'::JSONB,
    activities VARCHAR(50)[] NOT NULL,
    languages VARCHAR(10)[] DEFAULT '{}',
    
    -- Certifications
    certifications JSONB DEFAULT '[]'::JSONB,
    affiliations VARCHAR(100)[] DEFAULT '{}',
    insurance_info JSONB,
    
    -- Équipe
    staff JSONB DEFAULT '[]'::JSONB,
    
    -- Équipements
    facilities JSONB DEFAULT '[]'::JSONB,
    equipment_rental JSONB DEFAULT '[]'::JSONB,
    boats JSONB DEFAULT '[]'::JSONB,
    max_capacity INTEGER,
    
    -- Horaires
    opening_hours JSONB DEFAULT '{}'::JSONB,
    seasonal_closure JSONB,
    
    -- Tarification
    pricing_info TEXT,
    payment_methods VARCHAR(50)[] DEFAULT '{}',
    
    -- Vérification
    verification_status verification_status DEFAULT 'Pending',
    verified_at TIMESTAMP WITH TIME ZONE,
    verified_by UUID REFERENCES public.users(id),
    verification_documents JSONB DEFAULT '[]'::JSONB,
    
    -- Abonnement
    subscription_type subscription_status DEFAULT 'Free',
    subscription_start TIMESTAMP WITH TIME ZONE,
    subscription_end TIMESTAMP WITH TIME ZONE,
    subscription_features JSONB DEFAULT '[]'::JSONB,
    
    -- Statistiques
    average_rating DECIMAL(2,1) DEFAULT 0.0,
    total_ratings INTEGER DEFAULT 0,
    total_reviews INTEGER DEFAULT 0,
    total_bookings INTEGER DEFAULT 0,
    response_time_hours DECIMAL(5,1),
    response_rate DECIMAL(3,1),
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    is_active BOOLEAN DEFAULT true,
    is_featured BOOLEAN DEFAULT false,
    deleted_at TIMESTAMP WITH TIME ZONE
);

-- Table Shops (commerces)
CREATE TABLE IF NOT EXISTS public.shops (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    owner_id UUID NOT NULL REFERENCES public.users(id),
    
    -- Informations de base
    name VARCHAR(200) NOT NULL,
    slug VARCHAR(250) UNIQUE NOT NULL,
    shop_type VARCHAR(50) NOT NULL,
    
    -- Description
    description TEXT NOT NULL,
    specialties VARCHAR(100)[] DEFAULT '{}',
    
    -- Localisation
    location GEOGRAPHY(POINT, 4326) NOT NULL,
    address TEXT NOT NULL,
    city VARCHAR(100) NOT NULL,
    region VARCHAR(100),
    country VARCHAR(2) NOT NULL,
    postal_code VARCHAR(20),
    
    -- Contact
    phone VARCHAR(20),
    email VARCHAR(255),
    website TEXT,
    opening_hours JSONB DEFAULT '{}'::JSONB,
    
    -- Services
    services JSONB DEFAULT '[]'::JSONB,
    brands VARCHAR(100)[] DEFAULT '{}',
    online_shop BOOLEAN DEFAULT false,
    shipping_available BOOLEAN DEFAULT false,
    
    -- Vérification
    verification_status verification_status DEFAULT 'Pending',
    verified_at TIMESTAMP WITH TIME ZONE,
    verified_by UUID REFERENCES public.users(id),
    
    -- Abonnement
    subscription_type subscription_status DEFAULT 'Free',
    subscription_end TIMESTAMP WITH TIME ZONE,
    
    -- Statistiques
    average_rating DECIMAL(2,1) DEFAULT 0.0,
    total_ratings INTEGER DEFAULT 0,
    total_reviews INTEGER DEFAULT 0,
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    is_active BOOLEAN DEFAULT true,
    is_featured BOOLEAN DEFAULT false,
    deleted_at TIMESTAMP WITH TIME ZONE
);

-- Table Community Posts (articles de blog)
CREATE TABLE IF NOT EXISTS public.community_posts (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    author_id UUID NOT NULL REFERENCES public.users(id),
    
    -- Contenu
    title VARCHAR(200) NOT NULL,
    slug VARCHAR(250) UNIQUE NOT NULL,
    content TEXT NOT NULL,
    excerpt VARCHAR(500),
    
    -- Catégorisation
    category VARCHAR(50) NOT NULL,
    tags VARCHAR(50)[] DEFAULT '{}',
    related_spot_id UUID REFERENCES public.spots(id),
    related_structure_id UUID REFERENCES public.structures(id),
    
    -- Média
    cover_image_url TEXT,
    images JSONB DEFAULT '[]'::JSONB,
    
    -- Engagement
    likes_count INTEGER DEFAULT 0,
    comments_count INTEGER DEFAULT 0,
    shares_count INTEGER DEFAULT 0,
    views_count INTEGER DEFAULT 0,
    
    -- Modération
    moderation_status moderation_status DEFAULT 'Pending',
    moderated_by UUID REFERENCES public.users(id),
    moderated_at TIMESTAMP WITH TIME ZONE,
    featured BOOLEAN DEFAULT false,
    featured_until TIMESTAMP WITH TIME ZONE,
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    published_at TIMESTAMP WITH TIME ZONE,
    is_draft BOOLEAN DEFAULT true,
    is_active BOOLEAN DEFAULT true,
    deleted_at TIMESTAMP WITH TIME ZONE
);

-- Table Buddy Profiles (profils pour rencontres entre plongeurs)
CREATE TABLE IF NOT EXISTS public.buddy_profiles (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID UNIQUE NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    
    -- Statut
    is_active BOOLEAN DEFAULT false,
    visibility VARCHAR(50) DEFAULT 'nearby',
    
    -- Localisation
    current_location GEOGRAPHY(POINT, 4326),
    search_radius_km INTEGER DEFAULT 50,
    travel_dates JSONB DEFAULT '[]'::JSONB,
    
    -- Profil
    practice_info JSONB DEFAULT '{
        "activities": [],
        "experience_years": 0,
        "dive_count": 0,
        "favorite_conditions": [],
        "equipment_owned": [],
        "certifications": []
    }'::JSONB,
    
    -- Préférences
    preferences JSONB DEFAULT '{
        "buddy_type": ["practice", "learning", "exploration"],
        "experience_level": ["all"],
        "age_range": {"min": 18, "max": 99},
        "languages": [],
        "group_size": {"min": 1, "max": 4}
    }'::JSONB,
    
    -- Présentation
    bio TEXT,
    interests VARCHAR(100)[] DEFAULT '{}',
    availability VARCHAR(50),
    
    -- Matching
    last_active TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    match_score_boost INTEGER DEFAULT 0,
    
    -- Sécurité
    verified_diver BOOLEAN DEFAULT false,
    safety_notes TEXT,
    emergency_contact_shared BOOLEAN DEFAULT false,
    
    -- Statistiques
    total_matches INTEGER DEFAULT 0,
    successful_meetups INTEGER DEFAULT 0,
    response_rate DECIMAL(3,1),
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Table Buddy Matches
CREATE TABLE IF NOT EXISTS public.buddy_matches (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    profile1_id UUID NOT NULL REFERENCES public.buddy_profiles(id) ON DELETE CASCADE,
    profile2_id UUID NOT NULL REFERENCES public.buddy_profiles(id) ON DELETE CASCADE,
    
    -- Actions
    profile1_action VARCHAR(50),
    profile2_action VARCHAR(50),
    profile1_acted_at TIMESTAMP WITH TIME ZONE,
    profile2_acted_at TIMESTAMP WITH TIME ZONE,
    
    -- Match
    is_match BOOLEAN DEFAULT false,
    matched_at TIMESTAMP WITH TIME ZONE,
    match_score DECIMAL(3,2),
    
    -- Communication
    conversation_id UUID,
    first_message_at TIMESTAMP WITH TIME ZONE,
    
    -- Meetup
    meetup_proposed BOOLEAN DEFAULT false,
    meetup_date TIMESTAMP WITH TIME ZONE,
    meetup_location GEOGRAPHY(POINT, 4326),
    meetup_completed BOOLEAN DEFAULT false,
    meetup_feedback JSONB,
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    expires_at TIMESTAMP WITH TIME ZONE DEFAULT (NOW() + INTERVAL '30 days'),
    
    -- Contraintes
    CONSTRAINT unique_buddy_match UNIQUE (profile1_id, profile2_id),
    CONSTRAINT different_profiles CHECK (profile1_id != profile2_id)
);

-- Table Conversations
CREATE TABLE IF NOT EXISTS public.conversations (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    
    -- Type
    conversation_type conversation_type NOT NULL DEFAULT 'Direct',
    
    -- Participants
    participants UUID[] NOT NULL,
    
    -- Contexte
    context_type VARCHAR(50),
    context_id UUID,
    
    -- Dernier message
    last_message_id UUID,
    last_message_at TIMESTAMP WITH TIME ZONE,
    last_message_preview VARCHAR(200),
    
    -- Statut
    is_active BOOLEAN DEFAULT true,
    archived_by UUID[] DEFAULT '{}',
    muted_by UUID[] DEFAULT '{}',
    blocked_by UUID[] DEFAULT '{}',
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    
    -- Contraintes
    CONSTRAINT min_participants CHECK (array_length(participants, 1) >= 2)
);

-- Table Messages
CREATE TABLE IF NOT EXISTS public.messages (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    conversation_id UUID NOT NULL REFERENCES public.conversations(id) ON DELETE CASCADE,
    sender_id UUID NOT NULL REFERENCES public.users(id),
    
    -- Contenu
    content TEXT NOT NULL,
    message_type message_type DEFAULT 'Text',
    
    -- Statut
    is_read_by UUID[] DEFAULT '{}',
    is_edited BOOLEAN DEFAULT false,
    edited_at TIMESTAMP WITH TIME ZONE,
    is_deleted BOOLEAN DEFAULT false,
    deleted_at TIMESTAMP WITH TIME ZONE,
    
    -- Sécurité
    flagged_as_inappropriate BOOLEAN DEFAULT false,
    flagged_by UUID REFERENCES public.users(id),
    flagged_reason TEXT,
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    
    -- Contraintes
    CONSTRAINT content_length CHECK (char_length(content) <= 1000)
);

-- Table Bookings (réservations)
CREATE TABLE IF NOT EXISTS public.bookings (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    booking_number VARCHAR(20) UNIQUE NOT NULL,
    
    -- Parties
    customer_id UUID NOT NULL REFERENCES public.users(id),
    structure_id UUID NOT NULL REFERENCES public.structures(id),
    
    -- Service
    service_type VARCHAR(100) NOT NULL,
    service_details JSONB NOT NULL,
    
    -- Dates
    booking_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME,
    duration_minutes INTEGER,
    participants_count INTEGER NOT NULL DEFAULT 1,
    
    -- Tarification
    unit_price DECIMAL(10,2),
    total_price DECIMAL(10,2) NOT NULL,
    currency VARCHAR(3) DEFAULT 'EUR',
    payment_status payment_status DEFAULT 'Pending',
    payment_method VARCHAR(50),
    
    -- Statut
    booking_status booking_status DEFAULT 'Pending',
    confirmed_at TIMESTAMP WITH TIME ZONE,
    confirmed_by UUID REFERENCES public.users(id),
    cancelled_at TIMESTAMP WITH TIME ZONE,
    cancelled_by UUID REFERENCES public.users(id),
    cancellation_reason TEXT,
    
    -- Notes
    customer_notes TEXT,
    structure_notes TEXT,
    special_requirements TEXT,
    
    -- Communication
    conversation_id UUID REFERENCES public.conversations(id),
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    reminder_sent BOOLEAN DEFAULT false,
    
    -- Contraintes
    CONSTRAINT valid_dates CHECK (booking_date >= CURRENT_DATE),
    CONSTRAINT valid_participants CHECK (participants_count > 0 AND participants_count <= 50),
    CONSTRAINT valid_price CHECK (total_price >= 0)
);

-- Table Reviews (avis)
CREATE TABLE IF NOT EXISTS public.reviews (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    reviewer_id UUID NOT NULL REFERENCES public.users(id),
    
    -- Entité
    entity_type VARCHAR(50) NOT NULL,
    entity_id UUID NOT NULL,
    
    -- Notation
    rating INTEGER NOT NULL,
    rating_details JSONB DEFAULT '{}'::JSONB,
    
    -- Commentaire
    title VARCHAR(200),
    comment TEXT,
    
    -- Contexte
    visit_date DATE,
    booking_id UUID REFERENCES public.bookings(id),
    
    -- Photos
    images JSONB DEFAULT '[]'::JSONB,
    
    -- Réponse
    owner_response TEXT,
    owner_response_at TIMESTAMP WITH TIME ZONE,
    
    -- Modération
    moderation_status moderation_status DEFAULT 'Pending',
    moderated_by UUID REFERENCES public.users(id),
    moderated_at TIMESTAMP WITH TIME ZONE,
    
    -- Utilité
    helpful_count INTEGER DEFAULT 0,
    unhelpful_count INTEGER DEFAULT 0,
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    is_verified_booking BOOLEAN DEFAULT false,
    
    -- Contraintes
    CONSTRAINT valid_rating CHECK (rating >= 1 AND rating <= 5)
);

-- Table Favorites
CREATE TABLE IF NOT EXISTS public.favorites (
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    entity_type VARCHAR(50) NOT NULL,
    entity_id UUID NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    
    PRIMARY KEY (user_id, entity_type, entity_id)
);

-- Table Notifications
CREATE TABLE IF NOT EXISTS public.notifications (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES public.users(id) ON DELETE CASCADE,
    
    -- Contenu
    type VARCHAR(50) NOT NULL,
    title VARCHAR(200) NOT NULL,
    message TEXT NOT NULL,
    
    -- Contexte
    entity_type VARCHAR(50),
    entity_id UUID,
    action_url TEXT,
    
    -- Statut
    is_read BOOLEAN DEFAULT false,
    read_at TIMESTAMP WITH TIME ZONE,
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    expires_at TIMESTAMP WITH TIME ZONE DEFAULT (NOW() + INTERVAL '30 days')
);

-- Table Advertisements
CREATE TABLE IF NOT EXISTS public.advertisements (
    -- Identifiants
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    advertiser_id UUID REFERENCES public.users(id),
    
    -- Type
    ad_type VARCHAR(50) NOT NULL,
    title VARCHAR(200),
    content TEXT,
    image_url TEXT,
    click_url TEXT,
    
    -- Ciblage
    targeting JSONB DEFAULT '{
        "locations": [],
        "activities": [],
        "user_types": [],
        "age_range": null
    }'::JSONB,
    
    -- Placement
    placement VARCHAR(50) NOT NULL,
    position INTEGER,
    priority INTEGER DEFAULT 0,
    
    -- Budget
    budget_total DECIMAL(10,2),
    budget_daily DECIMAL(10,2),
    budget_spent DECIMAL(10,2) DEFAULT 0,
    cost_model VARCHAR(50),
    
    -- Statistiques
    impressions INTEGER DEFAULT 0,
    clicks INTEGER DEFAULT 0,
    conversions INTEGER DEFAULT 0,
    ctr DECIMAL(5,2) GENERATED ALWAYS AS 
        (CASE WHEN impressions > 0 THEN (clicks::DECIMAL / impressions * 100) ELSE 0 END) STORED,
    
    -- Planification
    start_date TIMESTAMP WITH TIME ZONE NOT NULL,
    end_date TIMESTAMP WITH TIME ZONE NOT NULL,
    
    -- Statut
    status VARCHAR(50) DEFAULT 'Pending',
    approved_by UUID REFERENCES public.users(id),
    approved_at TIMESTAMP WITH TIME ZONE,
    rejection_reason TEXT,
    
    -- Métadonnées
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    
    -- Contraintes
    CONSTRAINT valid_dates CHECK (end_date > start_date),
    CONSTRAINT valid_budget CHECK (
        (budget_total IS NULL OR budget_total > 0) AND
        (budget_daily IS NULL OR budget_daily > 0) AND
        (budget_spent >= 0)
    )
);

-- Table Audit Logs
CREATE TABLE IF NOT EXISTS public.audit_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES public.users(id),
    action VARCHAR(100) NOT NULL,
    entity_type VARCHAR(50),
    entity_id UUID,
    old_values JSONB,
    new_values JSONB,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- =====================================================
-- PARTIE 4: INDEX POUR OPTIMISATION
-- =====================================================

-- Index Users
CREATE INDEX idx_users_auth_id ON public.users(auth_id);
CREATE INDEX idx_users_email ON public.users(email);
CREATE INDEX idx_users_username ON public.users(username) WHERE username IS NOT NULL;
CREATE INDEX idx_users_location ON public.users USING GIST(default_location);
CREATE INDEX idx_users_account_type ON public.users(account_type);
CREATE INDEX idx_users_moderator ON public.users(moderator_status) WHERE moderator_status != 'None';
CREATE INDEX idx_users_active ON public.users(is_active) WHERE is_active = true;
CREATE INDEX idx_users_deleted ON public.users(deleted_at) WHERE deleted_at IS NOT NULL;

-- Index Spots
CREATE INDEX idx_spots_location ON public.spots USING GIST(location);
CREATE INDEX idx_spots_creator ON public.spots(creator_id);
CREATE INDEX idx_spots_validation ON public.spots(validation_status);
CREATE INDEX idx_spots_country ON public.spots(country);
CREATE INDEX idx_spots_difficulty ON public.spots(difficulty_level);
CREATE INDEX idx_spots_activities ON public.spots USING GIN(activities);
CREATE INDEX idx_spots_rating ON public.spots(average_rating DESC);
CREATE INDEX idx_spots_popularity ON public.spots(popularity_score DESC);
CREATE INDEX idx_spots_active_approved ON public.spots(is_active, validation_status) 
    WHERE is_active = true AND validation_status = 'Approved';
CREATE INDEX idx_spots_slug ON public.spots(slug);
CREATE INDEX idx_spots_search ON public.spots USING GIN(
    to_tsvector('french_unaccent', name || ' ' || COALESCE(description, ''))
);

-- Index Structures
CREATE INDEX idx_structures_location ON public.structures USING GIST(location);
CREATE INDEX idx_structures_owner ON public.structures(owner_id);
CREATE INDEX idx_structures_type ON public.structures(structure_type);
CREATE INDEX idx_structures_verification ON public.structures(verification_status);
CREATE INDEX idx_structures_activities ON public.structures USING GIN(activities);
CREATE INDEX idx_structures_rating ON public.structures(average_rating DESC);
CREATE INDEX idx_structures_featured ON public.structures(is_featured) WHERE is_featured = true;
CREATE INDEX idx_structures_active ON public.structures(is_active) WHERE is_active = true;
CREATE INDEX idx_structures_slug ON public.structures(slug);

-- Index Shops
CREATE INDEX idx_shops_location ON public.shops USING GIST(location);
CREATE INDEX idx_shops_owner ON public.shops(owner_id);
CREATE INDEX idx_shops_type ON public.shops(shop_type);
CREATE INDEX idx_shops_active ON public.shops(is_active) WHERE is_active = true;
CREATE INDEX idx_shops_slug ON public.shops(slug);

-- Index Community Posts
CREATE INDEX idx_posts_author ON public.community_posts(author_id);
CREATE INDEX idx_posts_category ON public.community_posts(category);
CREATE INDEX idx_posts_tags ON public.community_posts USING GIN(tags);
CREATE INDEX idx_posts_featured ON public.community_posts(featured) WHERE featured = true;
CREATE INDEX idx_posts_published ON public.community_posts(published_at DESC) WHERE is_draft = false;
CREATE INDEX idx_posts_slug ON public.community_posts(slug);

-- Index Buddy Profiles
CREATE INDEX idx_buddy_profiles_user ON public.buddy_profiles(user_id);
CREATE INDEX idx_buddy_profiles_location ON public.buddy_profiles USING GIST(current_location);
CREATE INDEX idx_buddy_profiles_active ON public.buddy_profiles(is_active, last_active DESC) 
    WHERE is_active = true;

-- Index Buddy Matches
CREATE INDEX idx_matches_profile1 ON public.buddy_matches(profile1_id);
CREATE INDEX idx_matches_profile2 ON public.buddy_matches(profile2_id);
CREATE INDEX idx_matches_is_match ON public.buddy_matches(is_match) WHERE is_match = true;
CREATE INDEX idx_matches_expires ON public.buddy_matches(expires_at);

-- Index Conversations
CREATE INDEX idx_conversations_participants ON public.conversations USING GIN(participants);
CREATE INDEX idx_conversations_last_message ON public.conversations(last_message_at DESC);
CREATE INDEX idx_conversations_active ON public.conversations(is_active) WHERE is_active = true;

-- Index Messages
CREATE INDEX idx_messages_conversation ON public.messages(conversation_id);
CREATE INDEX idx_messages_sender ON public.messages(sender_id);
CREATE INDEX idx_messages_created ON public.messages(created_at DESC);
CREATE INDEX idx_messages_unread ON public.messages(conversation_id) 
    WHERE is_deleted = false;

-- Index Bookings
CREATE INDEX idx_bookings_customer ON public.bookings(customer_id);
CREATE INDEX idx_bookings_structure ON public.bookings(structure_id);
CREATE INDEX idx_bookings_date ON public.bookings(booking_date);
CREATE INDEX idx_bookings_status ON public.bookings(booking_status);
CREATE INDEX idx_bookings_number ON public.bookings(booking_number);

-- Index Reviews
CREATE INDEX idx_reviews_entity ON public.reviews(entity_type, entity_id);
CREATE INDEX idx_reviews_reviewer ON public.reviews(reviewer_id);
CREATE INDEX idx_reviews_rating ON public.reviews(rating);
CREATE INDEX idx_reviews_created ON public.reviews(created_at DESC);
CREATE INDEX idx_reviews_moderation ON public.reviews(moderation_status) 
    WHERE moderation_status = 'Pending';

-- Index Favorites
CREATE INDEX idx_favorites_user ON public.favorites(user_id);
CREATE INDEX idx_favorites_entity ON public.favorites(entity_type, entity_id);

-- Index Notifications
CREATE INDEX idx_notifications_user ON public.notifications(user_id);
CREATE INDEX idx_notifications_unread ON public.notifications(user_id, is_read) 
    WHERE is_read = false;
CREATE INDEX idx_notifications_expires ON public.notifications(expires_at);

-- Index Advertisements
CREATE INDEX idx_ads_advertiser ON public.advertisements(advertiser_id);
CREATE INDEX idx_ads_status ON public.advertisements(status);
CREATE INDEX idx_ads_placement ON public.advertisements(placement);
CREATE INDEX idx_ads_active ON public.advertisements(status, start_date, end_date) 
    WHERE status = 'Active';

-- Index Audit Logs
CREATE INDEX idx_audit_user ON public.audit_logs(user_id);
CREATE INDEX idx_audit_action ON public.audit_logs(action);
CREATE INDEX idx_audit_entity ON public.audit_logs(entity_type, entity_id);
CREATE INDEX idx_audit_created ON public.audit_logs(created_at DESC);

-- =====================================================
-- PARTIE 5: FONCTIONS UTILITAIRES
-- =====================================================

-- Fonction pour mettre à jour automatiquement updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Fonction pour générer un slug unique
CREATE OR REPLACE FUNCTION generate_unique_slug(base_text TEXT, table_name TEXT)
RETURNS TEXT AS $$
DECLARE
    slug TEXT;
    counter INTEGER := 0;
    slug_exists BOOLEAN;
BEGIN
    -- Générer le slug de base
    slug := lower(unaccent(regexp_replace(base_text, '[^a-zA-Z0-9]+', '-', 'g')));
    slug := regexp_replace(slug, '^-+|-+$', '', 'g');
    
    -- Vérifier l'unicité
    EXECUTE format('SELECT EXISTS(SELECT 1 FROM %I WHERE slug = $1)', table_name) 
        INTO slug_exists USING slug;
    
    -- Ajouter un compteur si nécessaire
    WHILE slug_exists LOOP
        counter := counter + 1;
        slug := slug || '-' || counter;
        EXECUTE format('SELECT EXISTS(SELECT 1 FROM %I WHERE slug = $1)', table_name) 
            INTO slug_exists USING slug;
    END LOOP;
    
    RETURN slug;
END;
$$ LANGUAGE plpgsql;

-- Fonction pour calculer la distance entre deux points (en km)
CREATE OR REPLACE FUNCTION calculate_distance(lat1 DECIMAL, lon1 DECIMAL, lat2 DECIMAL, lon2 DECIMAL)
RETURNS DECIMAL AS $$
BEGIN
    RETURN ST_DistanceSphere(
        ST_MakePoint(lon1, lat1)::geography,
        ST_MakePoint(lon2, lat2)::geography
    ) / 1000;
END;
$$ LANGUAGE plpgsql;

-- Fonction pour rechercher les spots à proximité
CREATE OR REPLACE FUNCTION get_nearby_spots(
    lat DECIMAL, 
    lng DECIMAL, 
    radius_km INTEGER DEFAULT 50
)
RETURNS TABLE(
    id UUID,
    name VARCHAR,
    distance_km DECIMAL
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        s.id,
        s.name,
        ROUND((ST_DistanceSphere(
            s.location::geometry,
            ST_MakePoint(lng, lat)::geography::geometry
        ) / 1000)::DECIMAL, 2) as distance_km
    FROM public.spots s
    WHERE 
        s.is_active = true 
        AND s.validation_status = 'Approved'
        AND ST_DWithin(
            s.location::geography,
            ST_MakePoint(lng, lat)::geography,
            radius_km * 1000
        )
    ORDER BY distance_km;
END;
$$ LANGUAGE plpgsql;

-- Fonction pour calculer le score de popularité
CREATE OR REPLACE FUNCTION calculate_popularity_score(
    p_average_rating DECIMAL,
    p_total_ratings INTEGER,
    p_total_reviews INTEGER,
    p_total_favorites INTEGER,
    p_days_old INTEGER
)
RETURNS DECIMAL AS $$
DECLARE
    score DECIMAL := 0;
BEGIN
    score := (
        (COALESCE(p_average_rating, 0) * 20) +                    -- Max 100
        (LEAST(p_total_ratings, 100) * 0.5) +                     -- Max 50
        (LEAST(p_total_reviews, 50) * 1) +                        -- Max 50
        (LEAST(p_total_favorites, 100) * 0.3) +                   -- Max 30
        (CASE 
            WHEN p_days_old < 7 THEN 20                           -- Boost nouveau
            WHEN p_days_old < 30 THEN 10
            ELSE 0
        END)
    );
    
    RETURN ROUND(score, 2);
END;
$$ LANGUAGE plpgsql;

-- Fonction pour générer un numéro de réservation unique
CREATE OR REPLACE FUNCTION generate_booking_number()
RETURNS VARCHAR AS $$
DECLARE
    booking_number VARCHAR;
    number_exists BOOLEAN := true;
BEGIN
    WHILE number_exists LOOP
        booking_number := 'BK' || TO_CHAR(NOW(), 'YYMMDD') || 
                         LPAD(FLOOR(RANDOM() * 10000)::TEXT, 4, '0');
        
        SELECT EXISTS(SELECT 1 FROM public.bookings WHERE booking_number = booking_number)
        INTO number_exists;
    END LOOP;
    
    RETURN booking_number;
END;
$$ LANGUAGE plpgsql;

-- Fonction pour gérer les nouveaux utilisateurs depuis Supabase Auth
CREATE OR REPLACE FUNCTION handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO public.users (
        auth_id, 
        email, 
        created_at
    )
    VALUES (
        NEW.id, 
        NEW.email, 
        NOW()
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Fonction pour mettre à jour les statistiques des spots
CREATE OR REPLACE FUNCTION update_spot_stats()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_TABLE_NAME = 'reviews' THEN
        UPDATE public.spots
        SET 
            average_rating = (
                SELECT AVG(rating)::DECIMAL(2,1) 
                FROM public.reviews 
                WHERE entity_type = 'spot' 
                AND entity_id = NEW.entity_id
                AND moderation_status = 'Approved'
            ),
            total_ratings = (
                SELECT COUNT(*) 
                FROM public.reviews 
                WHERE entity_type = 'spot' 
                AND entity_id = NEW.entity_id
                AND moderation_status = 'Approved'
            ),
            total_reviews = (
                SELECT COUNT(*) 
                FROM public.reviews 
                WHERE entity_type = 'spot' 
                AND entity_id = NEW.entity_id
                AND comment IS NOT NULL
                AND moderation_status = 'Approved'
            )
        WHERE id = NEW.entity_id;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- PARTIE 6: TRIGGERS
-- =====================================================

-- Triggers pour updated_at
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON public.users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_spots_updated_at BEFORE UPDATE ON public.spots
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_structures_updated_at BEFORE UPDATE ON public.structures
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_shops_updated_at BEFORE UPDATE ON public.shops
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_community_posts_updated_at BEFORE UPDATE ON public.community_posts
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_bookings_updated_at BEFORE UPDATE ON public.bookings
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_conversations_updated_at BEFORE UPDATE ON public.conversations
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Trigger pour gérer les nouveaux utilisateurs Auth
CREATE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION handle_new_user();

-- Trigger pour mettre à jour les stats des spots après un review
CREATE TRIGGER update_spot_stats_on_review
    AFTER INSERT OR UPDATE ON public.reviews
    FOR EACH ROW 
    WHEN (NEW.entity_type = 'spot')
    EXECUTE FUNCTION update_spot_stats();

-- Trigger pour générer automatiquement le slug
CREATE OR REPLACE FUNCTION generate_slug_trigger()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.slug IS NULL OR NEW.slug = '' THEN
        NEW.slug := generate_unique_slug(NEW.name, TG_TABLE_NAME);
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER generate_spot_slug BEFORE INSERT ON public.spots
    FOR EACH ROW EXECUTE FUNCTION generate_slug_trigger();

CREATE TRIGGER generate_structure_slug BEFORE INSERT ON public.structures
    FOR EACH ROW EXECUTE FUNCTION generate_slug_trigger();

CREATE TRIGGER generate_shop_slug BEFORE INSERT ON public.shops
    FOR EACH ROW EXECUTE FUNCTION generate_slug_trigger();

CREATE TRIGGER generate_post_slug BEFORE INSERT ON public.community_posts
    FOR EACH ROW 
    WHEN (NEW.slug IS NULL OR NEW.slug = '')
    EXECUTE FUNCTION generate_slug_trigger();

-- Trigger pour générer le numéro de réservation
CREATE OR REPLACE FUNCTION set_booking_number()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.booking_number IS NULL THEN
        NEW.booking_number := generate_booking_number();
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER generate_booking_number BEFORE INSERT ON public.bookings
    FOR EACH ROW EXECUTE FUNCTION set_booking_number();

-- =====================================================
-- PARTIE 7: ROW LEVEL SECURITY (RLS)
-- =====================================================

-- Activer RLS sur toutes les tables
ALTER TABLE public.users ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.spots ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.structures ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.shops ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.community_posts ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.buddy_profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.buddy_matches ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.conversations ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.messages ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.bookings ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.reviews ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.favorites ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.notifications ENABLE ROW LEVEL SECURITY;

-- Policies pour Users
CREATE POLICY "Users can view active profiles" ON public.users
    FOR SELECT USING (
        is_active = true 
        OR auth.uid() = auth_id
    );

CREATE POLICY "Users can update own profile" ON public.users
    FOR UPDATE USING (auth.uid() = auth_id);

CREATE POLICY "Users can insert own profile" ON public.users
    FOR INSERT WITH CHECK (auth.uid() = auth_id);

-- Policies pour Spots
CREATE POLICY "View approved spots or own spots" ON public.spots
    FOR SELECT USING (
        (is_active = true AND validation_status = 'Approved')
        OR creator_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
    );

CREATE POLICY "Users can create spots" ON public.spots
    FOR INSERT WITH CHECK (
        creator_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
    );

CREATE POLICY "Users can update own pending spots" ON public.spots
    FOR UPDATE USING (
        creator_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
        AND validation_status IN ('Draft', 'Pending', 'RevisionRequested')
    );

-- Policies pour Messages
CREATE POLICY "Users can view messages in their conversations" ON public.messages
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.conversations c
            WHERE c.id = conversation_id
            AND (SELECT id FROM public.users WHERE auth_id = auth.uid()) = ANY(c.participants)
        )
    );

CREATE POLICY "Users can send messages in their conversations" ON public.messages
    FOR INSERT WITH CHECK (
        sender_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
        AND EXISTS (
            SELECT 1 FROM public.conversations c
            WHERE c.id = conversation_id
            AND sender_id = ANY(c.participants)
            AND c.is_active = true
        )
    );

-- Policies pour Bookings
CREATE POLICY "Users can view own bookings" ON public.bookings
    FOR SELECT USING (
        customer_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
        OR structure_id IN (
            SELECT id FROM public.structures 
            WHERE owner_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
        )
    );

CREATE POLICY "Users can create bookings" ON public.bookings
    FOR INSERT WITH CHECK (
        customer_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
    );

-- Policies pour Reviews
CREATE POLICY "Anyone can view approved reviews" ON public.reviews
    FOR SELECT USING (moderation_status = 'Approved');

CREATE POLICY "Users can create reviews" ON public.reviews
    FOR INSERT WITH CHECK (
        reviewer_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
    );

CREATE POLICY "Users can update own pending reviews" ON public.reviews
    FOR UPDATE USING (
        reviewer_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
        AND moderation_status = 'Pending'
    );

-- Policies pour Favorites
CREATE POLICY "Users can view own favorites" ON public.favorites
    FOR SELECT USING (
        user_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
    );

CREATE POLICY "Users can manage own favorites" ON public.favorites
    FOR ALL USING (
        user_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
    );

-- Policies pour Notifications
CREATE POLICY "Users can view own notifications" ON public.notifications
    FOR SELECT USING (
        user_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
    );

CREATE POLICY "Users can update own notifications" ON public.notifications
    FOR UPDATE USING (
        user_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
    );

-- Policies pour Buddy Profiles
CREATE POLICY "View active buddy profiles" ON public.buddy_profiles
    FOR SELECT USING (
        is_active = true
        OR user_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
    );

CREATE POLICY "Users 18+ can manage own buddy profile" ON public.buddy_profiles
    FOR ALL USING (
        user_id = (SELECT id FROM public.users WHERE auth_id = auth.uid())
        AND (SELECT birth_date FROM public.users WHERE auth_id = auth.uid()) <= CURRENT_DATE - INTERVAL '18 years'
    );

-- =====================================================
-- PARTIE 8: VUES UTILES
-- =====================================================

-- Vue pour les spots avec informations complètes
CREATE OR REPLACE VIEW v_spots_full AS
SELECT 
    s.*,
    u.username as creator_username,
    u.avatar_url as creator_avatar,
    ST_Y(s.location::geometry) as latitude,
    ST_X(s.location::geometry) as longitude,
    (SELECT COUNT(*) FROM public.favorites WHERE entity_type = 'spot' AND entity_id = s.id) as favorite_count,
    (SELECT COUNT(*) FROM public.reviews WHERE entity_type = 'spot' AND entity_id = s.id AND moderation_status = 'Approved') as review_count
FROM public.spots s
LEFT JOIN public.users u ON s.creator_id = u.id
WHERE s.is_active = true AND s.deleted_at IS NULL;

-- Vue pour les statistiques utilisateur
CREATE OR REPLACE VIEW v_user_stats AS
SELECT 
    u.id,
    u.username,
    u.expertise_level,
    (SELECT COUNT(*) FROM public.spots WHERE creator_id = u.id) as spots_created,
    (SELECT COUNT(*) FROM public.spots WHERE validated_by = u.id) as spots_validated,
    (SELECT COUNT(*) FROM public.reviews WHERE reviewer_id = u.id) as reviews_written,
    (SELECT COUNT(*) FROM public.community_posts WHERE author_id = u.id) as posts_written,
    (SELECT COUNT(*) FROM public.bookings WHERE customer_id = u.id) as bookings_made
FROM public.users u
WHERE u.is_active = true;

-- =====================================================
-- PARTIE 9: DONNÉES INITIALES
-- =====================================================

-- Insertion des types de spots par défaut (à adapter selon vos besoins)
-- Ces données seront insérées via l'interface d'administration

-- =====================================================
-- PARTIE 10: PERMISSIONS SUPABASE
-- =====================================================

-- Donner les permissions nécessaires au service role
GRANT ALL ON ALL TABLES IN SCHEMA public TO service_role;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO service_role;
GRANT ALL ON ALL FUNCTIONS IN SCHEMA public TO service_role;

-- Donner les permissions limitées au anon role
GRANT USAGE ON SCHEMA public TO anon;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO anon;

-- Permissions spécifiques pour authenticated role
GRANT USAGE ON SCHEMA public TO authenticated;
GRANT ALL ON ALL TABLES IN SCHEMA public TO authenticated;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO authenticated;

-- =====================================================
-- FIN DU SCRIPT
-- =====================================================

-- Vérification de la création
SELECT 
    table_name,
    COUNT(*) as column_count
FROM information_schema.columns
WHERE table_schema = 'public'
GROUP BY table_name
ORDER BY table_name;

-- Message de confirmation
DO $$
BEGIN
    RAISE NOTICE 'Database setup completed successfully!';
    RAISE NOTICE 'Tables created: %', (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public');
    RAISE NOTICE 'Indexes created: %', (SELECT COUNT(*) FROM pg_indexes WHERE schemaname = 'public');
    RAISE NOTICE 'RLS enabled on all tables';
END $$;