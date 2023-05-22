<?php

namespace App\Helpers;

use Illuminate\Support\Facades\DB;

class DatabaseHelper
{
    public static function truncateIfExists($table)
    {
        if (DB::getSchemaBuilder()->hasTable($table)) {
            DB::table($table)->truncate();
        }
    }

    public static function disableForeignKeyConstraints()
    {
        DB::statement('SET FOREIGN_KEY_CHECKS=0;');
    }

    public static function enableForeignKeyConstraints()
    {
        DB::statement('SET FOREIGN_KEY_CHECKS=1;');
    }
}
