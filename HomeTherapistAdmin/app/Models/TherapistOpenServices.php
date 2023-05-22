<?php

namespace App\Models;

use Backpack\CRUD\app\Models\Traits\CrudTrait;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Relations\Pivot;

class TherapistOpenServices extends Pivot
{
    use CrudTrait;
    use HasFactory;

    protected $table = 'therapist_open_services';
}
