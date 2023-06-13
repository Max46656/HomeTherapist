<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Support\Facades\DB;

class CreateLocationView extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        DB::statement("
            CREATE VIEW locations_view AS
            SELECT 'user' AS type, id, ST_GeomFromText(CONCAT('POINT(', longitude, ' ', latitude, ')')) AS location
            FROM users
            UNION
            SELECT 'order' AS type, id, ST_GeomFromText(CONCAT('POINT(', longitude, ' ', latitude, ')')) AS location
            FROM orders
            UNION
            SELECT 'appointment' AS type, id, ST_GeomFromText(CONCAT('POINT(', longitude, ' ', latitude, ')')) AS location
            FROM appointments
        ");
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        DB::statement("DROP VIEW IF EXISTS locations_view");
    }
}
