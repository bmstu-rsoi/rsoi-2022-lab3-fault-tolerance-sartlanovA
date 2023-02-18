--
-- PostgreSQL database dump
--

-- Dumped from database version 14.2
-- Dumped by pg_dump version 14.2

-- Started on 2022-12-16 00:10:13

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 210 (class 1259 OID 49212)
-- Name: cars; Type: TABLE; Schema: public; Owner: program
--

CREATE TABLE public.cars (
    id integer NOT NULL,
    car_uid uuid NOT NULL,
    brand character varying(80) NOT NULL,
    model character varying(80) NOT NULL,
    registration_number character varying(20) NOT NULL,
    power integer,
    price integer NOT NULL,
    type character varying(20),
    availability boolean NOT NULL,
    CONSTRAINT cars_type_check CHECK (((type)::text = ANY ((ARRAY['SEDAN'::character varying, 'SUV'::character varying, 'MINIVAN'::character varying, 'ROADSTER'::character varying])::text[])))
);


ALTER TABLE public.cars OWNER TO program;

--
-- TOC entry 209 (class 1259 OID 49211)
-- Name: cars_id_seq; Type: SEQUENCE; Schema: public; Owner: program
--

CREATE SEQUENCE public.cars_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.cars_id_seq OWNER TO program;

--
-- TOC entry 3323 (class 0 OID 0)
-- Dependencies: 209
-- Name: cars_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: program
--

ALTER SEQUENCE public.cars_id_seq OWNED BY public.cars.id;


--
-- TOC entry 3168 (class 2604 OID 49215)
-- Name: cars id; Type: DEFAULT; Schema: public; Owner: program
--

ALTER TABLE ONLY public.cars ALTER COLUMN id SET DEFAULT nextval('public.cars_id_seq'::regclass);



--
-- TOC entry 3316 (class 0 OID 49212)
-- Dependencies: 210
-- Data for Name: cars; Type: TABLE DATA; Schema: public; Owner: program
--

INSERT INTO public.cars (id, car_uid, brand, model, registration_number, power, price, type, availability) VALUES (1, '109b42f3-198d-4c89-9276-a7520a7120ab', 'Mercedes Benz', 'GLA 250', 'ЛО777Х799', 249, 3500, 'SEDAN', true);

--
-- TOC entry 3324 (class 0 OID 0)
-- Dependencies: 209
-- Name: cars_id_seq; Type: SEQUENCE SET; Schema: public; Owner: program
--

SELECT pg_catalog.setval('public.cars_id_seq', 1, false);

--
-- TOC entry 3171 (class 2606 OID 49220)
-- Name: cars cars_car_uid_key; Type: CONSTRAINT; Schema: public; Owner: program
--

ALTER TABLE ONLY public.cars
    ADD CONSTRAINT cars_car_uid_key UNIQUE (car_uid);


--
-- TOC entry 3173 (class 2606 OID 49218)
-- Name: cars cars_pkey; Type: CONSTRAINT; Schema: public; Owner: program
--

ALTER TABLE ONLY public.cars
    ADD CONSTRAINT cars_pkey PRIMARY KEY (id);


-- Completed on 2022-12-16 00:10:13

--
-- PostgreSQL database dump complete
--

